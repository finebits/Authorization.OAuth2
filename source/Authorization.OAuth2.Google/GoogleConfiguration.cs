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

namespace Finebits.Authorization.OAuth2.Google
{
    public class GoogleConfiguration : AuthConfiguration
    {
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        private const string RefreshTokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string RevokeTokenEndpoint = "https://oauth2.googleapis.com/revoke";
        private const string UserProfileEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo?alt=json";

        public GoogleConfiguration()
            : this(new Uri(AuthorizationEndpoint), new Uri(TokenEndpoint), new Uri(RefreshTokenEndpoint), new Uri(RevokeTokenEndpoint), new Uri(UserProfileEndpoint))
        { }

        public GoogleConfiguration(Uri authorizationEndpoint, Uri tokenEndpoint, Uri refreshEndpoint, Uri revokeEndpoint, Uri userProfileEndpoint)
            : base(authorizationEndpoint, tokenEndpoint, refreshEndpoint, revokeEndpoint, userProfileEndpoint)
        { }
    }
}
