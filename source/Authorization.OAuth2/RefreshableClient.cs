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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2
{
    public abstract partial class AuthorizationClient
    {
        protected class RefreshableClient : IRefreshable
        {
            private readonly AuthorizationClient _client;
            public Func<Token, NameValueCollection> RefreshPayloadCreator { get; set; }

            public RefreshableClient(AuthorizationClient client)
            {
                _client = client;
            }

            public async Task<AuthorizationToken> RefreshTokenAsync(Token token, CancellationToken cancellationToken = default)
            {
                if (token is null)
                {
                    throw new ArgumentNullException(nameof(token));
                }

                cancellationToken.ThrowIfCancellationRequested();

                TokenContent response = await _client.SendRequestAsync<TokenContent>(
                    endpoint: _client.Config.RefreshUri,
                    method: HttpMethod.Post,
                    token: token,
                    payload: RefreshPayloadCreator?.Invoke(token) ?? GetDefaultRefreshPayload(token),
                    headers: null,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                return new AuthorizationToken(
                    response.AccessToken,
                    response.RefreshToken,
                    response.TokenType,
                    TimeSpan.FromSeconds(response.ExpiresIn),
                    response.Scope
                    );
            }

            private NameValueCollection GetDefaultRefreshPayload(Token token)
            {
                return new RefreshPayload()
                {
                    ClientId = _client.Config.ClientId,
                    ClientSecret = _client.Config.ClientSecret,
                    RefreshToken = (token ?? throw new ArgumentNullException(nameof(token))).RefreshToken,
                }.GetCollection();
            }
        }

        protected class RefreshPayload
        {
            public string ClientId { get; set; }

            public string ClientSecret { get; set; }

            public string RefreshToken { get; set; }

            public string GrantType
            {
                get { return _grantType ?? RefreshTokenType; }
                set { _grantType = value; }
            }

            public NameValueCollection GetCollection()
            {
                NameValueCollection result = new NameValueCollection
                {
                    {"grant_type", GrantType},
                    {"client_id", ClientId},
                    {"refresh_token", RefreshToken},
                };

                if (!string.IsNullOrEmpty(ClientSecret))
                {
                    result.Add("client_secret", ClientSecret);
                }

                return result;
            }

            private string _grantType;
            private const string RefreshTokenType = "refresh_token";
        }
    }
}
