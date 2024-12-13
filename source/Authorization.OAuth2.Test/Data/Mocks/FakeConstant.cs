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

namespace Finebits.Authorization.OAuth2.Test.Data.Mocks
{
    internal static class FakeConstant
    {
        public const string Error = "fake-error";
        public const string ErrorDescription = "fake-error-description";
        public const string ErrorSubcode = "fake-error-subcode";

        public static class Credential
        {
            public const string TokenType = "Bearer";
            public const string AccessToken = "fake-access-token";
            public const string RefreshToken = "fake-refresh-token";
            public const string Scope = "fake-scope-1 fake-scope-2";
            public const string IdToken = "fake-id-token";

            public const string NewAccessToken = "fake-new-access-token";
            public const string NewRefreshToken = "fake-new-refresh-token";

            public const int ExpiresIn = 3600;
        }

        public static class UserProfile
        {
            public const string Id = "fake-id";
            public const string DisplayName = "fake-display-name";
            public const string Email = "fake@email";

            public static class Google
            {
                public const string Name = "fake-name";
                public const string FamilyName = "fake-family-name";
                public const string Picture = "https://google/avatar-uri";
                public const bool EmailVerified = true;
                public const string Locale = "fake-locale";
            }

            public static class Microsoft
            {
                public const string GivenName = "fake-givenName";
                public const string Surname = "fake-surname";
                public const string UserPrincipalName = "fake-userPrincipalName";
                public const string PreferredLanguage = "fake-preferredLanguage";
                public const string MobilePhone = "fake-mobilePhone";
                public const string JobTitle = "fake-jobTitle";
                public const string OfficeLocation = "fake-officeLocation";
            }

            public static class Outlook
            {
                public const string Alias = "fake-Alias";
                public const string MailboxGuid = "fake-MailboxGuid";
            }

            public const string Avatar = "fake-avatar-stream";
        }
    }
}
