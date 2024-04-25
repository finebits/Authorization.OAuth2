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

namespace Finebits.Authorization.OAuth2.Outlook
{
    public class OutlookConfiguration : AuthConfiguration
    {
        private const string UserProfileEndpoint = "https://outlook.office.com/api/v2.0/me";

        private const string DefaultTenant = "common";
        public OutlookAuthPrompt Prompt { get; set; } = OutlookAuthPrompt.SelectAccount;

        public OutlookConfiguration()
            : this(DefaultTenant)
        { }

        public OutlookConfiguration(string tenant)
             : this(GetAuthorizationUri(tenant), GetTokenUri(tenant), GetRefreshUri(tenant))
        { }

        public OutlookConfiguration(
            Uri authorizationEndpoint = null,
            Uri tokenEndpoint = null,
            Uri refreshEndpoint = null,
            Uri userProfileEndpoint = null)
            : base(authorizationEndpoint ?? GetAuthorizationUri(DefaultTenant),
                  tokenEndpoint ?? GetTokenUri(DefaultTenant),
                  refreshEndpoint ?? GetRefreshUri(DefaultTenant),
                  null,
                  userProfileEndpoint ?? new Uri(UserProfileEndpoint))
        { }

        public static string ConvertPromptToString(OutlookAuthPrompt prompt)
        {
            switch (prompt)
            {
                case OutlookAuthPrompt.Login:
                    return "login";
                case OutlookAuthPrompt.Consent:
                    return "consent";
                case OutlookAuthPrompt.SelectAccount:
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

    public enum OutlookAuthPrompt
    {
        None,
        Login,
        Consent,
        SelectAccount
    }
}
