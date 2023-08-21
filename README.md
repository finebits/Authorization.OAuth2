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

|Name|Latest version|Downloads|Code Coverage|
|:-|:-:|:-:|:-:|
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2-test-coverage.json&label=coverage) </sub> |
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2.AuthenticationBroker.Desktop-test-coverage.json&label=coverage) </sub> |
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2.Google-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Google) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2.Google?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Google) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2.Google)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Google) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2.Google-test-coverage.json&label=coverage) </sub> |
| <sub> [![name](https://img.shields.io/badge/Finebits.Authorization.OAuth2.Microsoft-blue)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Microsoft) </sub> | <sub> [![latest version](https://img.shields.io/nuget/v/Finebits.Authorization.OAuth2.Microsoft?logo=nuget&label)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Microsoft) </sub> | <sub> [![downloads](https://img.shields.io/nuget/dt/Finebits.Authorization.OAuth2.Microsoft)](https://www.nuget.org/packages/Finebits.Authorization.OAuth2.Microsoft) </sub> | <sub> ![Code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Authorization.OAuth2-main-Finebits.Authorization.OAuth2.Microsoft-test-coverage.json&label=coverage) </sub> |

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
```

### Example

```C#
using Finebits.Authorization.OAuth2.AuthenticationBroker;
using Finebits.Authorization.OAuth2.Google;
using Finebits.Authorization.OAuth2.Types;

if (!DesktopAuthenticationBroker.IsSupported)
{
    Console.WriteLine($"{typeof(DesktopAuthenticationBroker).FullName} is not supported.");
    return;
}

using var httpClient = new HttpClient();
var launcher = new WebBrowserLauncher();
var redirectURI = DesktopAuthenticationBroker.GetLoopbackUri();

var config = new GoogleConfiguration
{
    ClientId = "<client-id>",
    ClientSecret = "<client-secret>",
    RedirectUri = redirectURI,
    ScopeList = new[] { "<scope>", "..." }
};

var authClient = new GoogleAuthClient(httpClient, new DesktopAuthenticationBroker(launcher), config);

var token = await authClient.LoginAsync();
Console.WriteLine("Authorization operation is completed.");

var result = await authClient.RefreshTokenAsync(token);
Console.WriteLine("Refresh operation is completed.");

token = new AuthorizationToken(accessToken: result.AccessToken,
                               refreshToken: !string.IsNullOrEmpty(result.RefreshToken) ? result.RefreshToken : token.RefreshToken,
                               tokenType: result.TokenType,
                               expiresIn: result.ExpiresIn,
                               scope: result.Scope
                              );

await authClient.RevokeTokenAsync(token);
Console.WriteLine("Revoke operation is completed.");
```

## Links

- Google: 
  - [Using OAuth 2.0 to Access Google APIs](https://developers.google.com/identity/protocols/oauth2)
  - [OAuth 2.0 for Mobile & Desktop Apps](https://developers.google.com/identity/protocols/oauth2/native-app)
- Microsoft:
  - [OAuth 2.0 authorization code flow](https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow)
  - [Register an application with the Microsoft identity platform](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
