namespace OAuthWpfTest.Core.Models;

/// <summary>
/// Represents the authenticated user's information retrieved from Cognito.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// The user's display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The Cognito groups/roles assigned to the user.
    /// </summary>
    public IReadOnlyList<string> Roles { get; set; } = [];
}
