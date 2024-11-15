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

using System;

namespace Finebits.Authorization.OAuth2.Microsoft
{
    public class MicrosoftConfiguration : AuthConfiguration
    {
        private const string UserProfileEndpoint = "https://graph.microsoft.com/v1.0/me";
        private const string UserAvatarEndpoint = "https://graph.microsoft.com/v1.0/me/photo/$value";

        private const string DefaultTenant = "common";
        public MicrosoftAuthPrompt Prompt { get; set; } = MicrosoftAuthPrompt.SelectAccount;

        public Uri UserAvatarUri { get; protected set; }

        public MicrosoftConfiguration()
            : this(DefaultTenant)
        { }

        public MicrosoftConfiguration(string tenant)
             : this(GetAuthorizationUri(tenant), GetTokenUri(tenant), GetRefreshUri(tenant))
        { }

        public MicrosoftConfiguration(
            Uri authorizationEndpoint = null,
            Uri tokenEndpoint = null,
            Uri refreshEndpoint = null,
            Uri userProfileEndpoint = null,
            Uri userAvatarEndpoint = null)
            : base(authorizationEndpoint ?? GetAuthorizationUri(DefaultTenant),
                  tokenEndpoint ?? GetTokenUri(DefaultTenant),
                  refreshEndpoint ?? GetRefreshUri(DefaultTenant),
                  null,
                  userProfileEndpoint ?? new Uri(UserProfileEndpoint))
        {
            UserAvatarUri = userAvatarEndpoint ?? new Uri(UserAvatarEndpoint);
        }

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
                case MicrosoftAuthPrompt.None:
                    return "none";
                default:
                    return "none";
            }
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
