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

using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Google
{
    public class GoogleAuthorizationToken : AuthorizationToken
    {
        public string IdToken { get; private set; }

        public GoogleAuthorizationToken(string accessToken, string refreshToken, string tokenType, TimeSpan expiresIn, string scope, string idToken)
            : base(accessToken, refreshToken, tokenType, expiresIn, scope)
        {
            IdToken = idToken;
        }
    }
}
