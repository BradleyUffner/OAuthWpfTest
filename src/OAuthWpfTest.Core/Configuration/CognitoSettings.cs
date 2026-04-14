namespace OAuthWpfTest.Core.Configuration;

/// <summary>
/// Configuration settings for AWS Cognito authentication.
/// </summary>
public class CognitoSettings
{
    /// <summary>
    /// The Cognito domain URL (e.g., https://your-domain.auth.us-east-1.amazoncognito.com).
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// The Cognito App Client ID.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The redirect URI for the OAuth callback (e.g., http://localhost:7890/callback).
    /// </summary>
    public string RedirectUri { get; set; } = "http://localhost:7890/callback";

    /// <summary>
    /// The scopes to request during authentication.
    /// </summary>
    public string Scopes { get; set; } = "openid profile email";
}
