using OAuthWpfTest.Core.Models;
using OAuthWpfTest.Core.ViewModels;

namespace OAuthWpfTest.Tests;

public class MainWindowViewModelTests
{
    private readonly MockCognitoAuthService _mockAuthService;
    private readonly MainWindowViewModel _viewModel;

    public MainWindowViewModelTests()
    {
        _mockAuthService = new MockCognitoAuthService();
        _viewModel = new MainWindowViewModel(_mockAuthService);
    }

    [Fact]
    public void InitialState_IsNotLoggedIn()
    {
        Assert.False(_viewModel.IsLoggedIn);
        Assert.Null(_viewModel.CurrentUser);
        Assert.Equal("Please sign in to continue.", _viewModel.StatusMessage);
        Assert.Null(_viewModel.ErrorMessage);
        Assert.False(_viewModel.IsLoading);
    }

    [Fact]
    public async Task LoginCommand_Success_SetsCurrentUser()
    {
        var expectedUser = new UserInfo
        {
            Name = "John Doe",
            Email = "john@example.com",
            Roles = ["Admin", "User"]
        };
        _mockAuthService.SetupLoginResult(expectedUser);

        await _viewModel.LoginCommand.ExecuteAsync(null);

        Assert.True(_viewModel.IsLoggedIn);
        Assert.NotNull(_viewModel.CurrentUser);
        Assert.Equal("John Doe", _viewModel.CurrentUser!.Name);
        Assert.Equal("john@example.com", _viewModel.CurrentUser.Email);
        Assert.Equal(2, _viewModel.CurrentUser.Roles.Count);
        Assert.Contains("Admin", _viewModel.CurrentUser.Roles);
        Assert.Contains("User", _viewModel.CurrentUser.Roles);
        Assert.Equal("Welcome, John Doe!", _viewModel.StatusMessage);
        Assert.Null(_viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LoginCommand_Failure_SetsErrorMessage()
    {
        _mockAuthService.SetupLoginFailure(new InvalidOperationException("Authentication failed: test error"));

        await _viewModel.LoginCommand.ExecuteAsync(null);

        Assert.False(_viewModel.IsLoggedIn);
        Assert.Null(_viewModel.CurrentUser);
        Assert.NotNull(_viewModel.ErrorMessage);
        Assert.Contains("Sign-in failed", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LogoutCommand_ClearsCurrentUser()
    {
        // First login
        var user = new UserInfo { Name = "Jane", Email = "jane@example.com", Roles = ["User"] };
        _mockAuthService.SetupLoginResult(user);
        await _viewModel.LoginCommand.ExecuteAsync(null);
        Assert.True(_viewModel.IsLoggedIn);

        // Then logout
        await _viewModel.LogoutCommand.ExecuteAsync(null);

        Assert.False(_viewModel.IsLoggedIn);
        Assert.Null(_viewModel.CurrentUser);
        Assert.Equal("Please sign in to continue.", _viewModel.StatusMessage);
        Assert.Null(_viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LoginCommand_SetsIsLoadingDuringExecution()
    {
        var tcs = new TaskCompletionSource<UserInfo>();
        var loadingStates = new List<bool>();

        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainWindowViewModel.IsLoading))
                loadingStates.Add(_viewModel.IsLoading);
        };

        var user = new UserInfo { Name = "Test", Email = "test@test.com", Roles = [] };
        _mockAuthService.SetupLoginResult(user);

        await _viewModel.LoginCommand.ExecuteAsync(null);

        // Should have been set to true, then false
        Assert.Contains(true, loadingStates);
        Assert.False(_viewModel.IsLoading);
    }

    [Fact]
    public async Task LoginCommand_WithNoRoles_StillSucceeds()
    {
        var user = new UserInfo
        {
            Name = "No Roles User",
            Email = "noroles@example.com",
            Roles = []
        };
        _mockAuthService.SetupLoginResult(user);

        await _viewModel.LoginCommand.ExecuteAsync(null);

        Assert.True(_viewModel.IsLoggedIn);
        Assert.Empty(_viewModel.CurrentUser!.Roles);
    }
}
