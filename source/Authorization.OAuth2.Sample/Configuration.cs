﻿// ---------------------------------------------------------------------------- //
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
using Finebits.Authorization.OAuth2.AuthenticationBroker;
using Finebits.Authorization.OAuth2.AuthenticationBroker.Abstractions;
using Finebits.Authorization.OAuth2.Google;
using Finebits.Authorization.OAuth2.Microsoft;

namespace Finebits.Authorization.OAuth2.Sample;

partial class Program
{
    private static IAuthorizationClient GetGoogleAuthClient(HttpClient httpClient, IWebBrowserLauncher launcher, Uri redirectURI)
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
                "email"
            }
        };
        return new GoogleAuthClient(httpClient, new DesktopAuthenticationBroker(launcher), config);
    }

    private static IAuthorizationClient GetMicrosoftAuthClient(HttpClient httpClient, IWebBrowserLauncher launcher, Uri redirectURI)
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
        return new MicrosoftAuthClient(httpClient, new DesktopAuthenticationBroker(launcher), config);
    }

    private static IAuthorizationClient GetOffice365AuthClient(HttpClient httpClient, IWebBrowserLauncher launcher, Uri redirectURI)
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
        return new MicrosoftAuthClient(httpClient, new DesktopAuthenticationBroker(launcher), config);
    }
}
