# Authorization.OAuth2

![Target](https://img.shields.io/badge/dynamic/xml?label=Target&query=//TargetFramework[1]&url=https://raw.githubusercontent.com/finebits/Authorization.OAuth2/main/source/Authorization.OAuth2/Authorization.OAuth2.csproj)
[![License](https://img.shields.io/github/license/finebits/Authorization.OAuth2.svg)](https://github.com/finebits/Authorization.OAuth2/blob/main/LICENSE)

This is a .netstandard2 library that allows an application to support the OAuth 2.0 protocol.

## Build Status

|Branch|Status|Code Coverage|
|:-|:-:|:-:|
| **[main](https://github.com/finebits/Authorization.OAuth2/tree/main)** | <sub>[![Main branch: build and test](https://img.shields.io/github/actions/workflow/status/finebits/Authorization.OAuth2/build-and-test.yml?branch=main&logo=github&label=)](https://github.com/finebits/Authorization.OAuth2/actions/workflows/build-and-test.yml?query=branch%3Amain)</sub> | <sub>![Test coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-total-test-coverage.json)</sub> |
| **[develop](https://github.com/finebits/Authorization.OAuth2/tree/develop)** | <sub>[![Develop branch: build and test](https://img.shields.io/github/actions/workflow/status/finebits/Authorization.OAuth2/build-and-test.yml?branch=develop&logo=github&label=)](https://github.com/finebits/Authorization.OAuth2/actions/workflows/build-and-test.yml?query=branch%3Adevelop)</sub> | <sub>![Test coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-develop-total-test-coverage.json)</sub> |

## NuGet Packages

The following packages are published from this repository:

|Name|Latest version|Downloads|Code Coverage|
|:-|:-:|:-:|:-:|
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2-test-coverage.json&label=coverage) </sub> |
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop-test-coverage.json&label=coverage) </sub> |
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2.Google-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Google) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2.Google?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Google) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2.Google)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Google) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2.Google-test-coverage.json&label=coverage) </sub> |
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2.Microsoft-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Microsoft) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2.Microsoft?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Microsoft) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2.Microsoft)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Microsoft) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2.Microsoft-test-coverage.json&label=coverage) </sub> |
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2.Outlook-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Outlook) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2.Outlook?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Outlook) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2.Outlook)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Outlook) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2.Outlook-test-coverage.json&label=coverage) </sub> |

## Usage

### Prerequisites

- Register an application;
  - Google - [Enable APIs for your project](https://developers.google.com/identity/protocols/oauth2/native-app#prerequisites);
  - Microsoft - [Register an application](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app#register-an-application).
- Use the client ID, client secret and scopes of your application to configure the authorization client.

### Installation

Add packages using the following .NET CLI commands:

```powershell
dotnet add package Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop [--version <VERSION>] [--prerelease]
# and add required packages
dotnet add package Finebits.Authorization.OAuth2.Google [--version <VERSION>] [--prerelease]
dotnet add package Finebits.Authorization.OAuth2.Microsoft [--version <VERSION>] [--prerelease]
dotnet add package Finebits.Authorization.OAuth2.Outlook [--version <VERSION>] [--prerelease]
```

### Example

```C#
using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.AuthenticationBroker;
using Finebits.Authorization.OAuth2.Google;
using Finebits.Authorization.OAuth2.Types;

if (!DesktopAuthenticationBroker.IsSupported)
{
    Console.WriteLine($"{typeof(DesktopAuthenticationBroker).FullName} is not supported.");
    return;
}

using HttpClient httpClient = new();
WebBrowserLauncher launcher = new();
Uri redirectURI = DesktopAuthenticationBroker.GetLoopbackUri();

// Create <client_id> and <client_secret>
// Add required scopes
// Look at https://console.developers.google.com/apis/credentials
GoogleConfiguration config = new()
{
    ClientId = "<client-id>",
    ClientSecret = "<client-secret>",
    RedirectUri = redirectURI,
    ScopeList = new[] { "<scope>", "..." }
};

GoogleAuthClient authClient = new(httpClient, new DesktopAuthenticationBroker(launcher), config);

// The result contains token parameters which can be used to send service requests.
Token token = await authClient.LoginAsync();

// The result contains fresh token parameters.
Token freshToken = await authClient.RefreshTokenAsync(token);

token.Update(freshToken);

IUserProfile profile = await authClient.ReadProfileAsync(token);
Console.WriteLine($"Name: {profile.DisplayName}");

if (profile is IUserAvatar userAvatar)
{
    Console.WriteLine($"Avatar URI: {userAvatar.Avatar}");
}

Stream avatar = await authClient.LoadAvatarAsync(token);

await authClient.RevokeTokenAsync(token);
```

## Links

- Google: 
  - [Using OAuth 2.0 to Access Google APIs](https://developers.google.com/identity/protocols/oauth2)
  - [OAuth 2.0 for Mobile & Desktop Apps](https://developers.google.com/identity/protocols/oauth2/native-app)
- Microsoft:
  - [OAuth 2.0 authorization code flow](https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow)
  - [Register an application with the Microsoft identity platform](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
- Outlook:
  - [Compare Microsoft Graph and Outlook REST API endpoints](https://learn.microsoft.com/en-us/outlook/rest/compare-graph)
  - [Authenticate an IMAP, POP or SMTP connection using OAuth](https://learn.microsoft.com/en-us/exchange/client-developer/legacy-protocols/how-to-authenticate-an-imap-pop-smtp-application-by-using-oauth)
  - [Register an application with the Microsoft identity platform](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
