# OAuthWpfTest

A WPF application demonstrating AWS Cognito authentication using OAuth 2.0/OpenID Connect with the MVVM pattern.

## Features

- **AWS Cognito Login** – Sign in via the Cognito Hosted UI using Authorization Code flow with PKCE
- **User Info Display** – Shows the authenticated user's name, email, and Cognito group roles
- **MVVM Architecture** – Clean separation of concerns with Models, ViewModels, and Views
- **Cross-Platform Core** – Business logic in a .NET 9 class library; UI in a WPF project targeting .NET 9 (Windows)

## Project Structure

```
OAuthWpfTest/
├── src/
│   ├── OAuthWpfTest/              # WPF application (Views, App entry point)
│   │   ├── Views/                 # XAML converters
│   │   ├── MainWindow.xaml        # Main application window
│   │   ├── App.xaml               # Application definition
│   │   └── appsettings.json       # Cognito configuration
│   └── OAuthWpfTest.Core/         # Cross-platform class library
│       ├── Configuration/         # CognitoSettings
│       ├── Models/                # UserInfo
│       ├── Services/              # ICognitoAuthService, CognitoAuthService
│       └── ViewModels/            # MainWindowViewModel
└── tests/
    └── OAuthWpfTest.Tests/        # Unit tests (xUnit)
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- An AWS Cognito User Pool with:
  - An App Client configured (no client secret, for public clients)
  - A Cognito domain (Hosted UI enabled)
  - Allowed callback URL: `http://localhost:7890/callback`
  - Allowed sign-out URL (optional)

## Configuration

Edit `src/OAuthWpfTest/appsettings.json` with your Cognito settings:

```json
{
  "Cognito": {
    "Domain": "https://your-domain.auth.us-east-1.amazoncognito.com",
    "ClientId": "your-cognito-app-client-id",
    "RedirectUri": "http://localhost:7890/callback",
    "Scopes": "openid profile email"
  }
}
```

| Setting       | Description                                                       |
|---------------|-------------------------------------------------------------------|
| `Domain`      | Your Cognito User Pool domain URL                                 |
| `ClientId`    | The App Client ID from your Cognito User Pool                     |
| `RedirectUri` | The localhost callback URL (must match the Cognito App Client)    |
| `Scopes`      | OAuth scopes to request (`openid profile email` recommended)      |

## Build & Run

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the WPF application (Windows only)
dotnet run --project src/OAuthWpfTest

# Run tests (cross-platform)
dotnet test
```

## How It Works

1. The application starts and displays a **Sign In** button.
2. Clicking **Sign In** opens the system's default browser to the Cognito Hosted UI.
3. The user authenticates with Cognito (username/password, social login, etc.).
4. Cognito redirects back to `http://localhost:7890/callback` with an authorization code.
5. The application exchanges the code for tokens using PKCE.
6. The ID token claims are parsed to extract the user's **name**, **email**, and **roles** (Cognito groups).
7. The user information is displayed in the application window.

## Architecture

The application follows the **MVVM** (Model-View-ViewModel) pattern:

- **Model** (`UserInfo`) – Represents user data (name, email, roles)
- **ViewModel** (`MainWindowViewModel`) – Manages authentication state and commands
- **View** (`MainWindow.xaml`) – WPF UI with data bindings to the ViewModel
- **Service** (`CognitoAuthService`) – Handles the OIDC authentication flow

The `OAuthWpfTest.Core` library contains all non-UI code and targets `net9.0`, making it testable on any platform. The WPF project targets `net9.0-windows` and contains only the UI layer.

