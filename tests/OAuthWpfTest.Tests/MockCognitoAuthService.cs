using OAuthWpfTest.Core.Models;
using OAuthWpfTest.Core.Services;

namespace OAuthWpfTest.Tests;

/// <summary>
/// A mock implementation of ICognitoAuthService for testing.
/// </summary>
public class MockCognitoAuthService : ICognitoAuthService
{
    private UserInfo? _userToReturn;
    private Exception? _exceptionToThrow;

    public void SetupLoginResult(UserInfo userInfo)
    {
        _userToReturn = userInfo;
        _exceptionToThrow = null;
    }

    public void SetupLoginFailure(Exception exception)
    {
        _exceptionToThrow = exception;
        _userToReturn = null;
    }

    public Task<UserInfo> LoginAsync()
    {
        if (_exceptionToThrow is not null)
            throw _exceptionToThrow;

        return Task.FromResult(_userToReturn ?? new UserInfo());
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }
}
