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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2
{
    public abstract partial class AuthorizationClient
    {
        protected class ProfileReader<TContent> : IProfileReader
            where TContent : IInvalidResponse
        {
            private readonly AuthorizationClient _client;
            public Func<TContent, IUserProfile> UserProfileCreator { get; set; }

            public ProfileReader(AuthorizationClient client)
            {
                _client = client;
            }

            public async Task<IUserProfile> ReadProfileAsync(Token token, CancellationToken cancellationToken = default)
            {
                if (token is null)
                {
                    throw new ArgumentNullException(nameof(token));
                }

                cancellationToken.ThrowIfCancellationRequested();

                var response = await _client.SendEmptyRequestAsync<TContent>(
                    endpoint: _client.Config.UserProfileUri,
                    method: HttpMethod.Get,
                    token: token,
                    headers: null,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                return (UserProfileCreator?.Invoke(response)) ?? (response is IUserProfile profile ? profile : null);
            }
        }
    }
}
