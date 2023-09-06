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

using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2
{
    public abstract partial class AuthorizationClient : AuthorizationRefreshableClient, IRefreshable, IRevocable
    {
        protected AuthorizationClient(HttpClient httpClient, IAuthenticationBroker broker, AuthConfiguration config)
            : base(httpClient, broker, config)
        { }

        public virtual async Task RevokeTokenAsync(Token token, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await SendRequestAsync<EmptyContent>(
                endpoint: Config.RevokeUri,
                payload: GetRevokePayload(token),
                headers: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        protected abstract NameValueCollection GetRevokePayload(Token token);
    }
}
