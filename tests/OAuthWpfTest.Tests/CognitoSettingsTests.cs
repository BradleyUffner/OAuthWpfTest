using OAuthWpfTest.Core.Configuration;

namespace OAuthWpfTest.Tests;

public class CognitoSettingsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var settings = new CognitoSettings();

        Assert.Equal(string.Empty, settings.Domain);
        Assert.Equal(string.Empty, settings.ClientId);
        Assert.Equal("http://localhost:7890/callback", settings.RedirectUri);
        Assert.Equal("openid profile email", settings.Scopes);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var settings = new CognitoSettings
        {
            Domain = "https://test.auth.us-east-1.amazoncognito.com",
            ClientId = "test-client-id",
            RedirectUri = "http://localhost:8080/callback",
            Scopes = "openid email"
        };

        Assert.Equal("https://test.auth.us-east-1.amazoncognito.com", settings.Domain);
        Assert.Equal("test-client-id", settings.ClientId);
        Assert.Equal("http://localhost:8080/callback", settings.RedirectUri);
        Assert.Equal("openid email", settings.Scopes);
    }
}
