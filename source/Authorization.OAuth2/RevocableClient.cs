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
        protected class RevocableClient : IRevocable
        {
            private readonly AuthorizationClient _client;
            public Func<Credential, NameValueCollection> RevokePayloadCreator { get; set; }

            public RevocableClient(AuthorizationClient client)
            {
                _client = client;
            }

            public virtual async Task RevokeAsync(Credential credential, CancellationToken cancellationToken = default)
            {
                if (credential is null)
                {
                    throw new ArgumentNullException(nameof(credential));
                }

                cancellationToken.ThrowIfCancellationRequested();

                await _client.SendRequestAsync<EmptyContent>(
                    endpoint: _client.Config.RevokeUri,
                    method: HttpMethod.Post,
                    credential: credential,
                    payload: RevokePayloadCreator?.Invoke(credential) ?? GetDefaultRevokePayload(credential),
                    headers: null,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            private static NameValueCollection GetDefaultRevokePayload(Credential _)
            {
                return [];
            }
        }
    }
}
