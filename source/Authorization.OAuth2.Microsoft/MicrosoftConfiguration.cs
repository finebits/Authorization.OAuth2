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

using System;

namespace Finebits.Authorization.OAuth2.Microsoft
{
    public class MicrosoftConfiguration : AuthConfiguration
    {
        //private const string RevokeTokenEndpoint = "https://graph.microsoft.com/v1.0/me/revokeSignInSessions";
        private const string DefaultTenant = "common";
        public MicrosoftAuthPrompt Prompt { get; set; } = MicrosoftAuthPrompt.SelectAccount;

        public MicrosoftConfiguration()
            : this(DefaultTenant)
        { }

        public MicrosoftConfiguration(string tenant)
             : this(GetAuthorizationUri(tenant), GetTokenUri(tenant), GetRefreshUri(tenant))
        { }

        public MicrosoftConfiguration(Uri authorizationEndpoint, Uri tokenEndpoint, Uri refreshEndpoint)
            : base(authorizationEndpoint, tokenEndpoint, refreshEndpoint, null)
        { }

        public static string ConvertPromptToString(MicrosoftAuthPrompt prompt)
        {
            switch (prompt)
            {
                case MicrosoftAuthPrompt.Login:
                    return "login";
                case MicrosoftAuthPrompt.Consent:
                    return "consent";
                case MicrosoftAuthPrompt.SelectAccount:
                    return "select_account";
            }

            return "none";
        }

        private static Uri GetAuthorizationUri(string tenant)
        {
            return new Uri($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize");
        }

        private static Uri GetTokenUri(string tenant)
        {
            return new Uri($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token");
        }

        private static Uri GetRefreshUri(string tenant)
        {
            return GetTokenUri(tenant);
        }
    }

    public enum MicrosoftAuthPrompt
    {
        None,
        Login,
        Consent,
        SelectAccount
    }
}
