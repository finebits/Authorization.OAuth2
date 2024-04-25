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

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.AuthenticationBroker;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Sample;

partial class Program
{
    private static async Task Main(string[] _)
    {
        try
        {
            using var httpClient = new HttpClient();
            var launcher = new WebBrowserLauncher();
            var redirectURI = DesktopAuthenticationBroker.GetLoopbackUri();

            Console.WriteLine("Welcome to OAuth2.Sample.");

            if (!DesktopAuthenticationBroker.IsSupported)
            {
                throw new InvalidOperationException($"{typeof(DesktopAuthenticationBroker).FullName} is not supported.");
            }

            Console.Write("""

                1. Google
                2. Microsoft
                3. Office365

                Select option (default is 1): 
                """);

            var authClient = Console.ReadLine() switch
            {
                "2" => GetMicrosoftAuthClient(httpClient, launcher, redirectURI),
                "3" => GetOffice365AuthClient(httpClient, launcher, redirectURI),
                _ => GetGoogleAuthClient(httpClient, launcher, redirectURI),
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

            var token = await authClient.LoginAsync(cts.Token).ConfigureAwait(false);
            PrintToken(token, "Login response");

            var freshToken = await RefreshTokenAsync(authClient, token).ConfigureAwait(false);
            PrintToken(token, "Refresh response");
            token.Update(freshToken);

            var profile = await ReadProfileAsync(authClient, token).ConfigureAwait(false);
            PrintProfile(profile, "User profile");

            await LoadUserAvatarAsync(authClient, token, "./avatar.jpg").ConfigureAwait(false);

            await RevokeTokenAsync(authClient, token).ConfigureAwait(false);
        }
        catch (AuthorizationBrokerResultException propEx)
        {
            Console.WriteLine($"""

                AuthorizationInvalidBrokerResultException:
                Error: {propEx.Error}
                ErrorDescription: {propEx.ErrorDescription}
                """);
        }
        catch (AuthorizationInvalidResponseException responseException) when (responseException.ResponseDetails is IMicrosoftInvalidResponse microsoftResponse)
        {
            Console.WriteLine($"""

                AuthorizationInvalidResponseException(IMicrosoftInvalidResponse):
                Error: {microsoftResponse.ErrorReason}
                ErrorDescription: {microsoftResponse.ErrorDescription}
                Code: {microsoftResponse.ResponseError.Code}
                Message: {microsoftResponse.ResponseError.Message}
                RequestDate: {microsoftResponse.ResponseError.InnerError.RequestDate}
                RequestId: {microsoftResponse.ResponseError.InnerError.RequestId}
                ClientRequestId: {microsoftResponse.ResponseError.InnerError.ClientRequestId}
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

    private static void PrintToken(Types.AuthorizationToken? authToken, string header)
    {
        if (authToken is null)
        {
            Console.WriteLine($"{header}: null");
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
    }

    private static void PrintProfile(IUserProfile? profile, string header)
    {
        if (profile is null)
        {
            Console.WriteLine($"{header}: null");
            return;
        }

        Console.WriteLine($"""

                {header}:
                {nameof(IUserProfile.Id)}: {profile.Id}
                {nameof(IUserProfile.Email)}: {profile.Email}
                {nameof(IUserProfile.DisplayName)}: {profile.DisplayName}
                {nameof(IUserAvatar.Avatar)}: {((profile is IUserAvatar avatar) ? avatar.Avatar : "link is missing")}
                {((profile is IUserAvatarLoader) ? "Avatar can be loaded" : string.Empty)}

                """);
    }

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

    private static async Task<IUserProfile?> ReadProfileAsync(IAuthorizationClient client, Token token)
    {
        IUserProfile? profile = null;

        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (client is IProfileReader profileReader)
        {
            profile = await profileReader.ReadProfileAsync(token).ConfigureAwait(false);
            Console.WriteLine("Read Profile operation is completed.");
        }

        return profile;
    }

    private static async Task LoadUserAvatarAsync(IAuthorizationClient client, Token token, string name)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (client is IUserAvatarLoader avatarLoader)
        {
            using var avatar = await avatarLoader.LoadAvatarAsync(token).ConfigureAwait(false);
            using var fileAvatar = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write);

            fileAvatar.SetLength(0);
            await avatar.CopyToAsync(fileAvatar).ConfigureAwait(false);
        }
    }
}
