// ---------------------------------------------------------------------------- //
//                                                                              //
//   Copyright 2024 Finebits (https://finebits.com/)                            //
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

using System.IdentityModel.Tokens.Jwt;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.AuthenticationBroker;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.Types;

using Microsoft.IdentityModel.Tokens;

namespace Finebits.Authorization.OAuth2.Sample;

internal partial class Program
{
    private static async Task Main(string[] _)
    {
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            using HttpClient httpClient = new();
            WebBrowserLauncher launcher = new();
            Uri redirectURI = DesktopAuthenticationBroker.GetLoopbackUri();

            WriteColorLine("Welcome to OAuth2.Sample.", ConsoleColor.White);

            if (!DesktopAuthenticationBroker.IsSupported)
            {
                throw new InvalidOperationException($"{typeof(DesktopAuthenticationBroker).FullName} is not supported.");
            }

            Console.Write("""

                1. Google
                2. Microsoft
                3. Outlook

                Select option (default is 1): 
                """);

            IAuthorizationClient authClient = Console.ReadLine() switch
            {
                "2" => GetMicrosoftAuthClient(httpClient, launcher, redirectURI),
                "3" => GetOutlookAuthClient(httpClient, launcher, redirectURI),
                _ => GetGoogleAuthClient(httpClient, launcher, redirectURI),
            };

            using CancellationTokenSource cts = new(TimeSpan.FromMinutes(5));

            AuthCredential credential = await authClient.LoginAsync(cts.Token).ConfigureAwait(false);
            Console.WriteLine();
            WriteColorLine("Login operation is completed.", ConsoleColor.Green);
            PrintCredential(credential, "Login response");

            Console.WriteLine();
            PrintTokenPayload(credential);

            Console.WriteLine();
            AuthCredential? fresh = await RefreshAsync(authClient, credential).ConfigureAwait(false);
            PrintCredential(credential, "Refresh response");
            credential.Update(fresh);

            Console.WriteLine();
            IUserProfile? profile = await ReadProfileAsync(authClient, credential).ConfigureAwait(false);
            PrintProfile(profile, "User profile");

            Console.WriteLine();
            await LoadUserAvatarAsync(authClient, credential, "./avatar.jpg").ConfigureAwait(false);

            Console.WriteLine();
            await RevokeAsync(authClient, credential).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            WriteError(exception);
        }
    }

    private static void PrintCredential(Types.AuthCredential? credential, string header)
    {
        if (credential is null)
        {
            return;
        }

        WriteColorLine($"{header}:", ConsoleColor.White);

        Console.WriteLine($"""
                TokenType: {credential.TokenType ?? "null"}
                AccessToken: {PrintToken(credential.AccessToken)}
                RefreshToken: {PrintToken(credential.RefreshToken)}
                IdToken: {PrintToken(credential.IdToken)}
                ExpiresIn: {credential.ExpiresIn}
                Scope: {credential.Scope ?? "null"}
                """);

        static string PrintToken(string token)
        {
            const int PrintLength = 8;
            return $"{(token.Length > PrintLength ? $"{token[0..PrintLength]}..." : "<empty>")}";
        }
    }

    private static void PrintProfile(IUserProfile? profile, string header)
    {
        if (profile is null)
        {
            return;
        }

        WriteColorLine($"{header}:", ConsoleColor.White);

        Console.WriteLine($"""
                {nameof(IUserProfile.Id)}: {profile.Id}
                {nameof(IUserProfile.Email)}: {profile.Email}
                {nameof(IUserProfile.DisplayName)}: {profile.DisplayName}
                {nameof(IUserAvatar.Avatar)}: {((profile is IUserAvatar avatar) ? avatar.Avatar : "link is missing")}
                """);
    }

    private static async Task<Types.AuthCredential?> RefreshAsync(IAuthorizationClient client, Credential credential)
    {
        if (client is IRefreshable refreshClient)
        {
            AuthCredential result = await refreshClient.RefreshAsync(credential).ConfigureAwait(false);
            WriteColorLine("Refresh operation is completed.", ConsoleColor.Green);
            return result;
        }
        else
        {
            WriteColorLine("Refresh operation unavailable.", ConsoleColor.Yellow);
        }

        return null;
    }

    private static async Task RevokeAsync(IAuthorizationClient client, Credential credential)
    {
        if (credential is null)
        {
            throw new ArgumentNullException(nameof(credential));
        }

        if (client is IRevocable revokeClient)
        {
            await revokeClient.RevokeAsync(credential).ConfigureAwait(false);
            WriteColorLine("Revoke operation is completed.", ConsoleColor.Green);
        }
        else
        {
            WriteColorLine("Revoke operation unavailable.", ConsoleColor.Yellow);
        }
    }

    private static async Task<IUserProfile?> ReadProfileAsync(IAuthorizationClient client, Credential credential)
    {
        IUserProfile? profile = null;

        if (credential is null)
        {
            throw new ArgumentNullException(nameof(credential));
        }

        if (client is IProfileReader profileReader)
        {
            profile = await profileReader.ReadProfileAsync(credential).ConfigureAwait(false);
            WriteColorLine("Read Profile operation is completed.", ConsoleColor.Green);
        }
        else
        {
            WriteColorLine("Read Profile operation unavailable.", ConsoleColor.Yellow);
        }

        return profile;
    }

    private static async Task LoadUserAvatarAsync(IAuthorizationClient client, Credential credential, string name)
    {
        if (credential is null)
        {
            throw new ArgumentNullException(nameof(credential));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (client is IUserAvatarLoader avatarLoader)
        {
            try
            {
                using Stream avatar = await avatarLoader.LoadAvatarAsync(credential).ConfigureAwait(false);
                using FileStream fileAvatar = new(name, FileMode.OpenOrCreate, FileAccess.Write);

                fileAvatar.SetLength(0);
                await avatar.CopyToAsync(fileAvatar).ConfigureAwait(false);
                WriteColorLine("Load Avatar operation is completed.", ConsoleColor.Green);
            }
            catch (AuthorizationException exception)
            {
                WriteError(exception, ConsoleColor.Yellow);
            }
        }
        else
        {
            WriteColorLine("Load Avatar operation unavailable.", ConsoleColor.Yellow);
        }
    }

    private static void PrintTokenPayload(AuthCredential credential)
    {
        if (credential is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(credential.IdToken))
        {
            WriteColorLine("Read IdToken operation unavailable.", ConsoleColor.Yellow);
            return;
        }

        JwtSecurityTokenHandler handler = new();
        SecurityToken securityToken = handler.ReadToken(credential.IdToken);

        if (securityToken is JwtSecurityToken jwtToken)
        {
            WriteColorLine("IdToken Payload:", ConsoleColor.White);

            foreach (KeyValuePair<string, object> item in jwtToken.Payload)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
    }

    private static void WriteError(Exception exception, ConsoleColor color = ConsoleColor.Red)
    {
        switch (exception)
        {
            case AuthorizationBrokerResultException brokerException:
            {
                WriteColorLine($"""

                        AuthorizationInvalidBrokerResultException:
                        Error: {brokerException.Error}
                        ErrorDescription: {brokerException.ErrorDescription}
                        """, color);
                break;
            }
            case AuthorizationInvalidResponseException responseException when responseException.ResponseDetails is IMicrosoftInvalidResponse microsoftResponse:
            {
                WriteColorLine($"""

                        AuthorizationInvalidResponseException(IMicrosoftInvalidResponse):
                        Error: {microsoftResponse.ErrorReason}
                        ErrorDescription: {microsoftResponse.ErrorDescription}
                        Code: {microsoftResponse.ResponseError.Code}
                        Message: {microsoftResponse.ResponseError.Message}
                        RequestDate: {microsoftResponse.ResponseError.InnerError.RequestDate}
                        RequestId: {microsoftResponse.ResponseError.InnerError.RequestId}
                        ClientRequestId: {microsoftResponse.ResponseError.InnerError.ClientRequestId}

                        Message: {responseException.Message}
                        InnerException.Message: {responseException.InnerException?.Message}
                        """, color);
                break;
            }
            case AuthorizationInvalidResponseException responseException when responseException.ResponseDetails is IOutlookInvalidResponse outlookResponse:
            {
                WriteColorLine($"""

                        AuthorizationInvalidResponseException(IOutlookInvalidResponse):
                        Error: {outlookResponse.ErrorReason}
                        ErrorDescription: {outlookResponse.ErrorDescription}
                        Code: {outlookResponse.ResponseError.Code}
                        Message: {outlookResponse.ResponseError.Message}
                        RequestDate: {outlookResponse.ResponseError.InnerError.RequestDate}
                        RequestId: {outlookResponse.ResponseError.InnerError.RequestId}
                        ErrorUrl: {outlookResponse.ResponseError.InnerError.ErrorUrl}

                        Message: {responseException.Message}
                        InnerException.Message: {responseException.InnerException?.Message}
                        """, color);
                break;
            }
            case AuthorizationInvalidResponseException responseException:
            {
                WriteColorLine($"""

                        AuthorizationInvalidResponseException:
                        Error: {responseException.ErrorReason}
                        ErrorDescription: {responseException.ErrorDescription}

                        Message: {responseException.Message}
                        InnerException.Message: {responseException.InnerException?.Message}
                        """, color);
                break;
            }
            case AuthorizationException authException when authException.InnerException is not null:
            {
                WriteColorLine($"""

                            AuthorizationException:
                            Message: {authException.Message}
                            InnerException.Message: {authException.InnerException.Message}
                            """, color);
                break;
            }
            default:
            {
                WriteColorLine($"""

                        Exception:
                        Message: {exception?.Message}
                        """, color);
                break;
            }
        }
    }

    private static void WriteColorLine(string text, ConsoleColor color)
    {
        ConsoleColor defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = defaultColor;
    }
}
