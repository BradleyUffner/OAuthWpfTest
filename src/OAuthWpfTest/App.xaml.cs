using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using OAuthWpfTest.Core.Configuration;
using OAuthWpfTest.Core.Services;
using OAuthWpfTest.Core.ViewModels;

namespace OAuthWpfTest;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var cognitoSettings = new CognitoSettings();
        configuration.GetSection("Cognito").Bind(cognitoSettings);

        ICognitoAuthService authService = new CognitoAuthService(cognitoSettings);
        var viewModel = new MainWindowViewModel(authService);

        var mainWindow = new MainWindow(viewModel);
        mainWindow.Show();
    }
}
