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
using System.Collections.Specialized;
using System.Text.Json.Serialization;

using Finebits.Authorization.OAuth2.Abstractions;

namespace Finebits.Authorization.OAuth2
{
    public abstract partial class AuthorizationClient
    {
        protected class AuthProperties
        {
            public string State { get; set; }
            public string CodeChallenge { get; set; }
            public string CodeChallengeMethod { get; set; }
            public string CodeVerifier { get; set; }
        }

        protected class TokenPayload
        {
            public string ClientId { get; set; }

            public string ClientSecret { get; set; }

            public string Code { get; set; }

            public string CodeVerifier { get; set; }

            public Uri RedirectUri { get; set; }

            public string GrantType
            {
                get { return _grantType ?? AuthorizationCodeType; }
                set { _grantType = value; }
            }

            public NameValueCollection GetCollection()
            {
                var result = new NameValueCollection
                {
                    {"grant_type", GrantType},
                    {"code", Code},
                    {"client_id", ClientId},
                    {"code_verifier", CodeVerifier},
                    {"redirect_uri", RedirectUri.ToString()},
                };

                if (!string.IsNullOrEmpty(ClientSecret))
                {
                    result.Add("client_secret", ClientSecret);
                }

                return result;
            }

            private string _grantType;
            private const string AuthorizationCodeType = "authorization_code";
        }

        protected internal class EmptyContent : IInvalidResponse
        {
            [JsonInclude]
            [JsonPropertyName("error_description")]
            public string ErrorDescription { get; private set; }

            [JsonInclude]
            [JsonPropertyName("error")]
            public string ErrorReason { get; private set; }
        }

        protected class TokenContent : EmptyContent
        {
            [JsonInclude]
            [JsonPropertyName("access_token")]
            public string AccessToken { get; private set; } = string.Empty;

            [JsonInclude]
            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; private set; } = string.Empty;

            [JsonInclude]
            [JsonPropertyName("token_type")]
            public string TokenType { get; private set; } = string.Empty;

            [JsonInclude]
            [JsonPropertyName("expires_in")]
            public ulong ExpiresIn { get; private set; } = 0;

            [JsonInclude]
            [JsonPropertyName("scope")]
            public string Scope { get; private set; } = string.Empty;
        }
    }
}
