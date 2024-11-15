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

using System.Collections.Specialized;
using System.Text.Json.Serialization;

using Finebits.Authorization.OAuth2.Abstractions;

namespace Finebits.Authorization.OAuth2.Google
{
    public partial class GoogleAuthClient
    {
        protected class GoogleTokenContent : TokenContent
        {
            [JsonInclude]
            [JsonPropertyName("id_token")]
            public string IdToken { get; private set; }
        }

        protected class GoogleProfileContent : IInvalidResponse
        {
            [JsonInclude]
            [JsonPropertyName("sub")]
            public string Id { get; private set; }

            [JsonInclude]
            [JsonPropertyName("name")]
            public string Name { get; private set; }

            [JsonInclude]
            [JsonPropertyName("given_name")]
            public string GivenName { get; private set; }

            [JsonInclude]
            [JsonPropertyName("family_name")]
            public string FamilyName { get; private set; }

            [JsonInclude]
            [JsonPropertyName("picture")]
            public string Picture { get; private set; }

            [JsonInclude]
            [JsonPropertyName("email")]
            public string Email { get; private set; }

            [JsonInclude]
            [JsonPropertyName("email_verified")]
            public bool IsEmailVerified { get; private set; }

            [JsonInclude]
            [JsonPropertyName("locale")]
            public string Locale { get; private set; }

            [JsonInclude]
            [JsonPropertyName("error_description")]
            public string ErrorDescription { get; private set; }

            [JsonInclude]
            [JsonPropertyName("error")]
            public string ErrorReason { get; private set; }
        }

        protected class RevokePayload
        {
            public string RefreshToken { get; set; }

            public NameValueCollection GetCollection()
            {
                return new NameValueCollection
                {
                    {"token", RefreshToken}
                };
            }
        }
    }
}
