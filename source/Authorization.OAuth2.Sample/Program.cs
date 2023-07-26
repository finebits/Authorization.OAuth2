// ---------------------------------------------------------------------------- //
//                                                                              //
//   Copyright 2023 Finebits (https://finebits.com/)                            //
//                                                                              //
//   Licensed under the Apache License, Version 2.0 (the "License"),            //
//   you may not use this file except in compliance with the License.           //
//   You may obtain a copy of the License at                                    //
//                                                                              //
//       http://www.apache.org/licenses/LICENSE-2.0                             //
//                                                                              //
//   Unless required by applicable law or agreed to in writing, software        //
//   distributed under the License is distributed on an "AS IS" BASIS,          //
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.   //
//   See the License for the specific language governing permissions and        //
//   limitations under the License.                                             //
//                                                                              //
// ---------------------------------------------------------------------------- //

using System.Net;
using System.Net.Sockets;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Brokers;
using Finebits.Authorization.OAuth2.Brokers.Abstractions;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.Google;
using Finebits.Authorization.OAuth2.Microsoft;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Sample;

class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    private static async Task Main(string[] args)
    {
        var arguments = string.Join(", ", args);
        arguments = string.IsNullOrEmpty(arguments) ? "empty" : arguments;

        Console.WriteLine($"Hello, World!\nCommand line: {arguments}");

        var port = GetRandomUnusedPort();

        try
        {
            using var httpClient = new HttpClient();
            var launcher = new WebBrowserLauncher();
            using var listener = new WebBrowserHttpListener();
            var redirectURI = new Uri($"http://{IPAddress.Loopback}:{port}/");


            Console.Write("""

                1. Google
                2. Microsoft
                3. Office365

                Select option (default is 1): 
                """);

            var authClient = Console.ReadLine() switch
            {
                "2" => GetMicrosoftAuthClient(httpClient, launcher, listener, redirectURI),
                "3" => GetOffice365AuthClient(httpClient, launcher, listener, redirectURI),
                _ => GetGoogleAuthClient(httpClient, launcher, listener, redirectURI),
            };

            var token = await authClient.LoginAsync().ConfigureAwait(false);
            PrintToken(token, "Login response");

            var newToken = await RefreshTokenAsync(authClient, token).ConfigureAwait(false);
            PrintToken(token, "Refresh response");
            token = UpdateToken(token, newToken);

            newToken = await RefreshTokenAsync(authClient, token).ConfigureAwait(false);
            PrintToken(token, "Refresh response");
            token = UpdateToken(token, newToken);

            await RevokeTokenAsync(authClient, token).ConfigureAwait(false);

            newToken = await RefreshTokenAsync(authClient, token).ConfigureAwait(false);
            PrintToken(token, "Refresh response");
        }
        catch (AuthorizationInvalidBrokerResultException propEx)
        {
            Console.WriteLine($"""

                Exception:
                AuthenticationPropertiesException.Error: {propEx.Error}
                AuthenticationPropertiesException.ErrorDescription: {propEx.ErrorDescription}
                """);
        }
        catch (AuthorizationException authEx) when (authEx.InnerException is not null)
        {
            Console.WriteLine(authEx.InnerException.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static AuthorizationToken UpdateToken(AuthorizationToken? oldToken, AuthorizationToken? newToken)
    {
        if (newToken is null)
        {
            throw new ArgumentNullException(nameof(newToken));
        }

        if (oldToken is null)
        {
            throw new ArgumentNullException(nameof(oldToken));
        }

        return new AuthorizationToken(
                accessToken: newToken.AccessToken,
                refreshToken: !string.IsNullOrEmpty(newToken.RefreshToken) ? newToken.RefreshToken : oldToken.RefreshToken,
                tokenType: newToken.TokenType,
                expiresIn: newToken.ExpiresIn,
                scope: newToken.Scope
            );
    }

    private static void PrintToken(Types.AuthorizationToken? authToken, string header)
    {
        if (authToken is null)
        {
            Console.WriteLine($" {header}: authToken is null");
            return;
        }

        Console.WriteLine($"""

                {header}:
                AccessToken: {authToken.AccessToken[0..8]}...
                RefreshToken: {authToken.RefreshToken[0..8]}...
                TokenType: {authToken.TokenType ?? "null"}
                ExpiresIn: {authToken.ExpiresIn}
                Scope: {authToken.Scope ?? "null"}
                """);

        if (authToken is GoogleAuthorizationToken googleToken)
        {
            Console.WriteLine($"IdToken: {googleToken.IdToken[0..8]}...");
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    private static async Task<Types.AuthorizationToken?> RefreshTokenAsync(IAuthorizationClient client, Token token)
    {
        if (client is IRefreshable refreshClient)
        {
            var result = await refreshClient.RefreshTokenAsync(token).ConfigureAwait(false);
            Console.WriteLine("Refresh operation is completed.");
            return result;
        }

        return null;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    private static async Task RevokeTokenAsync(IAuthorizationClient client, Token token)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (client is IRevocable revokeClient)
        {
            await revokeClient.RevokeTokenAsync(token).ConfigureAwait(false);
            Console.WriteLine("Revoke operation is completed.");
        }
    }

    private static IAuthorizationClient GetGoogleAuthClient(HttpClient httpClient, IWebBrowserLauncher launcher, AuthenticationListener listener, Uri redirectURI)
    {
        // Create <client_id> and <client_secret>: https://console.developers.google.com/apis/credentials
        // You can add additional scopes if necessary.
        var config = new GoogleConfiguration
        {
            ClientId = "<client_id>",
            ClientSecret = "<client_secret>",
            RedirectUri = redirectURI,
            ScopeList = new[]
            {
                "profile",
                "email",
            }
        };
        return new GoogleAuthClient(httpClient, new WebBrowserAuthenticationBroker(launcher, listener), config);
    }

    private static IAuthorizationClient GetMicrosoftAuthClient(HttpClient httpClient, IWebBrowserLauncher launcher, AuthenticationListener listener, Uri redirectURI)
    {
        // https://learn.microsoft.com/en-us/graph/auth-register-app-v2#register-an-application
        // Create <client_id>: https://portal.azure.com/
        // You can add additional scopes if necessary.
        var config = new MicrosoftConfiguration
        {
            ClientId = "<client_id>",
            RedirectUri = redirectURI,
            ScopeList = new[]
            {
                "offline_access",
                "https://graph.microsoft.com/.default",
            }
        };
        return new MicrosoftAuthClient(httpClient, new WebBrowserAuthenticationBroker(launcher, listener), config);
    }

    private static IAuthorizationClient GetOffice365AuthClient(HttpClient httpClient, IWebBrowserLauncher launcher, AuthenticationListener listener, Uri redirectURI)
    {
        // https://learn.microsoft.com/en-us/graph/auth-register-app-v2#register-an-application
        // Create <client_id>: https://portal.azure.com/
        // You can add additional scopes if necessary.
        var config = new MicrosoftConfiguration
        {
            ClientId = "<client_id>",
            RedirectUri = redirectURI,
            ScopeList = new[]
            {
                "offline_access",
            }
        };
        return new MicrosoftAuthClient(httpClient, new WebBrowserAuthenticationBroker(launcher, listener), config);
    }

    private static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
