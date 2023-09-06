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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Exceptions;

namespace Finebits.Authorization.OAuth2.Messages
{
    internal class NetworkClient : Finebits.Network.RestClient.Client
    {
        internal NetworkClient(HttpClient httpClient)
            : base(httpClient, null)
        { }

        internal async Task<TContent> SendRequestAsync<TContent>(
            Uri endpoint,
            NameValueCollection payload,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            using (var message = new NetworkMessage<TContent>(endpoint, payload, headers))
            {
                try
                {
                    await SendAsync(message, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    IInvalidResponse content = null;

                    if (message.Response != null)
                    {
                        content = message.Response.Content;
                    }
                    throw new AuthorizationInvalidResponseException(content, ex);
                }

                return GetContent(message.Response);
            }
        }

        internal async Task<TContent> SendRequestAsync<TContent>(
            Uri endpoint,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            using (var message = new EmptyNetworkMessage<TContent>(endpoint, headers))
            {
                try
                {
                    await SendAsync(message, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    IInvalidResponse content = null;

                    if (message.Response != null)
                    {
                        content = message.Response.Content;
                    }
                    throw new AuthorizationInvalidResponseException(content, ex);
                }

                return GetContent(message.Response);
            }
        }

        private static T GetContent<T>(Network.RestClient.JsonResponse<T> response)
            where T : IInvalidResponse
        {
            if (response is null || (response.Content == null && typeof(T) != typeof(BaseAuthorizationClient.EmptyContent)))
            {
                throw new AuthorizationEmptyResponseException("Response is empty.");
            }

            return string.IsNullOrEmpty(response.Content?.ErrorReason)
                ? (response.Content is ICloneable cloneable) ? (T)cloneable.Clone() : response.Content
                : throw new AuthorizationInvalidResponseException(response.Content);
        }
    }
}
