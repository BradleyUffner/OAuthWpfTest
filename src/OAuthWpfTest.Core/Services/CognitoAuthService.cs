using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using OAuthWpfTest.Core.Configuration;
using OAuthWpfTest.Core.Models;

namespace OAuthWpfTest.Core.Services;

/// <summary>
/// Implements AWS Cognito authentication using OIDC Authorization Code flow with PKCE.
/// Opens the system browser for login and listens on a local HTTP server for the callback.
/// </summary>
public class CognitoAuthService : ICognitoAuthService
{
    private readonly OidcClient _oidcClient;

    public CognitoAuthService(CognitoSettings settings)
    {
        var options = new OidcClientOptions
        {
            Authority = settings.Domain,
            ClientId = settings.ClientId,
            RedirectUri = settings.RedirectUri,
            Scope = settings.Scopes,
            Browser = new SystemBrowser(settings.RedirectUri),
            Policy = new Policy
            {
                Discovery = new IdentityModel.Client.DiscoveryPolicy
                {
                    ValidateEndpoints = false
                }
            }
        };

        _oidcClient = new OidcClient(options);
    }

    public async Task<UserInfo> LoginAsync()
    {
        var result = await _oidcClient.LoginAsync(new LoginRequest());

        if (result.IsError)
        {
            throw new InvalidOperationException($"Authentication failed: {result.Error}");
        }

        return ExtractUserInfo(result.User);
    }

    public Task LogoutAsync()
    {
        // For a desktop app, clearing local state is sufficient.
        // Optionally, navigate to the Cognito logout endpoint.
        return Task.CompletedTask;
    }

    private static UserInfo ExtractUserInfo(ClaimsPrincipal user)
    {
        var name = user.FindFirst("name")?.Value
                   ?? user.FindFirst("cognito:username")?.Value
                   ?? user.FindFirst(ClaimTypes.Name)?.Value
                   ?? "Unknown";

        var email = user.FindFirst("email")?.Value
                    ?? user.FindFirst(ClaimTypes.Email)?.Value
                    ?? "Unknown";

        var roles = user.FindAll("cognito:groups")
            .Concat(user.FindAll(ClaimTypes.Role))
            .Select(c => c.Value)
            .Distinct()
            .ToList();

        return new UserInfo
        {
            Name = name,
            Email = email,
            Roles = roles
        };
    }
}

/// <summary>
/// A browser implementation that opens the system default browser
/// and listens on a local HTTP server for the OAuth callback.
/// </summary>
internal sealed class SystemBrowser : IBrowser
{
    private readonly string _redirectUri;

    public SystemBrowser(string redirectUri)
    {
        _redirectUri = redirectUri;
    }

    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        var listener = new HttpListener();

        // Ensure the listener prefix ends with a path that matches the redirect URI
        var uri = new Uri(_redirectUri);
        var prefix = $"http://localhost:{uri.Port}{uri.AbsolutePath}";
        if (!prefix.EndsWith('/'))
            prefix += "/";

        listener.Prefixes.Add(prefix);
        listener.Start();

        try
        {
            // Open the system browser with the authorization URL
            OpenBrowser(options.StartUrl);

            // Wait for the callback
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMinutes(5));

            var context = await listener.GetContextAsync().WaitAsync(cts.Token);
            var rawUrl = context.Request.Url?.ToString() ?? string.Empty;

            // Send a response to the browser
            var response = context.Response;
            var responseBody = System.Text.Encoding.UTF8.GetBytes("""
                <html>
                <head><title>Login Successful</title></head>
                <body style="font-family: sans-serif; text-align: center; padding-top: 50px;">
                    <h1>&#10003; Login Successful</h1>
                    <p>You can close this browser window and return to the application.</p>
                </body>
                </html>
                """);
            response.ContentType = "text/html";
            response.ContentLength64 = responseBody.Length;
            await response.OutputStream.WriteAsync(responseBody, cancellationToken);
            response.Close();

            return new BrowserResult
            {
                ResultType = BrowserResultType.Success,
                Response = rawUrl
            };
        }
        catch (OperationCanceledException)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.Timeout,
                Error = "Login timed out. Please try again."
            };
        }
        finally
        {
            listener.Stop();
        }
    }

    private static void OpenBrowser(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
    }
}
