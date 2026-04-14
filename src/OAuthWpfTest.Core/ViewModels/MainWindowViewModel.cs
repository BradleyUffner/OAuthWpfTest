using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OAuthWpfTest.Core.Models;
using OAuthWpfTest.Core.Services;

namespace OAuthWpfTest.Core.ViewModels;

/// <summary>
/// Main window view model managing authentication state and user information display.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly ICognitoAuthService _authService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoggedIn))]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [NotifyCanExecuteChangedFor(nameof(LogoutCommand))]
    private UserInfo? _currentUser;

    [ObservableProperty]
    private string _statusMessage = "Please sign in to continue.";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    public bool IsLoggedIn => CurrentUser is not null;

    public MainWindowViewModel(ICognitoAuthService authService)
    {
        _authService = authService;
    }

    private bool CanLogin() => !IsLoggedIn && !IsLoading;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            StatusMessage = "Opening browser for sign-in...";

            var userInfo = await _authService.LoginAsync();

            CurrentUser = userInfo;
            StatusMessage = $"Welcome, {userInfo.Name}!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Sign-in failed: {ex.Message}";
            StatusMessage = "Sign-in failed. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanLogout() => IsLoggedIn && !IsLoading;

    [RelayCommand(CanExecute = nameof(CanLogout))]
    private async Task LogoutAsync()
    {
        try
        {
            IsLoading = true;
            await _authService.LogoutAsync();

            CurrentUser = null;
            StatusMessage = "Please sign in to continue.";
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Sign-out failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
