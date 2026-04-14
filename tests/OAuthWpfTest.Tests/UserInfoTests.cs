using OAuthWpfTest.Core.Models;

namespace OAuthWpfTest.Tests;

public class UserInfoTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var userInfo = new UserInfo();

        Assert.Equal(string.Empty, userInfo.Name);
        Assert.Equal(string.Empty, userInfo.Email);
        Assert.Empty(userInfo.Roles);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var userInfo = new UserInfo
        {
            Name = "Test User",
            Email = "test@example.com",
            Roles = ["Admin", "Editor"]
        };

        Assert.Equal("Test User", userInfo.Name);
        Assert.Equal("test@example.com", userInfo.Email);
        Assert.Equal(2, userInfo.Roles.Count);
        Assert.Contains("Admin", userInfo.Roles);
        Assert.Contains("Editor", userInfo.Roles);
    }

    [Fact]
    public void Roles_DefaultsToEmptyList()
    {
        var userInfo = new UserInfo();
        Assert.NotNull(userInfo.Roles);
        Assert.IsAssignableFrom<IReadOnlyList<string>>(userInfo.Roles);
    }
}
