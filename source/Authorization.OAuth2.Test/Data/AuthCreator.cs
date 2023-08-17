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

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Google;
using Finebits.Authorization.OAuth2.Microsoft;

namespace Finebits.Authorization.OAuth2.Test.Data
{
    enum AuthClientType
    {
        Google,
        Microsoft
    }

    internal class AuthCreator
    {
        internal static IAuthorizationClient CreateAuthClient(AuthClientType type, HttpClient? httpClient, IAuthenticationBroker? broker, AuthConfiguration? config)
        {
            return type switch
            {
                AuthClientType.Microsoft => CreateMicrosoftAuthClient(httpClient, broker, config),
                AuthClientType.Google => CreateGoogleAuthClient(httpClient, broker, config),
                _ => throw new NotImplementedException(),
            };
        }

        internal static IAuthorizationClient CreateGoogleAuthClient(HttpClient? httpClient, IAuthenticationBroker? broker, AuthConfiguration? config)
        {
            return new GoogleAuthClient(httpClient, broker, config as GoogleConfiguration);
        }

        internal static IAuthorizationClient CreateMicrosoftAuthClient(HttpClient? httpClient, IAuthenticationBroker? broker, AuthConfiguration? config)
        {
            return new MicrosoftAuthClient(httpClient, broker, config as MicrosoftConfiguration);
        }

        internal static AuthConfiguration CreateConfig(AuthClientType type)
        {
            return type switch
            {
                AuthClientType.Microsoft => CreateMicrosoftConfig(),
                AuthClientType.Google => CreateGoogleConfig(),
                _ => throw new NotImplementedException(),
            };
        }

        internal static AuthConfiguration CreateGoogleConfig()
        {
            var host = new Uri("https://google");

            return new GoogleConfiguration(
                new Uri(host, "auth-uri"),
                new Uri(host, "token-uri"),
                new Uri(host, "refresh-uri"),
                new Uri(host, "revoke-uri"))
            {
                ClientId = "fake-google-client-id",
                ClientSecret = "fake-google-client-secret",
                RedirectUri = new Uri("https://redirect"),
                ScopeList = new[] { "fake-scope" }
            };
        }

        internal static AuthConfiguration CreateMicrosoftConfig()
        {
            var host = new Uri("https://microsoft");

            return new MicrosoftConfiguration(
                new Uri(host, "auth-uri"),
                new Uri(host, "token-uri"),
                new Uri(host, "refresh-uri"))
            {
                ClientId = "fake-microsoft-client-id",
                RedirectUri = new Uri("https://redirect"),
                ScopeList = new[] { "fake-scope" }
            };
        }
    }
}
