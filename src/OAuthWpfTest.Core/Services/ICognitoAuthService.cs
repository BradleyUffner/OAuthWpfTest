using OAuthWpfTest.Core.Models;

namespace OAuthWpfTest.Core.Services;

/// <summary>
/// Interface for AWS Cognito authentication operations.
/// </summary>
public interface ICognitoAuthService
{
    /// <summary>
    /// Initiates the login flow using the system browser and returns the authenticated user's info.
    /// </summary>
    /// <returns>The authenticated user's information.</returns>
    Task<UserInfo> LoginAsync();

    /// <summary>
    /// Logs the user out by clearing tokens and optionally revoking the session.
    /// </summary>
    Task LogoutAsync();
}
