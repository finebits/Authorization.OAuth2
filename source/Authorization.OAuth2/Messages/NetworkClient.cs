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
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Network.RestClient;

namespace Finebits.Authorization.OAuth2.Messages
{
    internal class NetworkClient : Finebits.Network.RestClient.Client
    {
        internal NetworkClient(HttpClient httpClient)
            : base(httpClient, null)
        { }

        internal async Task<TContent> SendRequestAsync<TContent>(
            Uri endpoint,
            HttpMethod method,
            NameValueCollection payload,
            HeaderCollection headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            using (var message = new NetworkMessage<TContent>(endpoint, method, payload, headers))
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
            HttpMethod method,
            HeaderCollection headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            using (var message = new EmptyNetworkMessage<TContent>(endpoint, method, headers))
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

        internal async Task<Stream> SendRequestAsync(
            Uri endpoint,
            HttpMethod method,
            HeaderCollection headers,
            CancellationToken cancellationToken)
        {
            using (var message = new StreamNetworkMessage(endpoint, method, headers))
            {
                try
                {
                    await SendAsync(message, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (HttpRequestException ex)
                {
                    //ToDo: add multi-content to RestClient
                    var emptyContent = new AuthorizationClient.EmptyContent();
                    throw new AuthorizationInvalidResponseException(emptyContent, ex);
                }
                catch (Exception ex)
                {
                    throw new AuthorizationInvalidResponseException("Stream can't be load", ex);
                }

                var result = new MemoryStream();
                message.Response?.Stream?.CopyTo(result);
                result.Position = 0;

                return result;
            }
        }

        private static T GetContent<T>(Network.RestClient.JsonResponse<T> response)
            where T : IInvalidResponse
        {
            if (response is null || (response.Content == null && typeof(T) != typeof(AuthorizationClient.EmptyContent)))
            {
                throw new AuthorizationEmptyResponseException("Response is empty.");
            }

            return string.IsNullOrEmpty(response.Content?.ErrorReason)
                ? (response.Content is ICloneable cloneable) ? (T)cloneable.Clone() : response.Content
                : throw new AuthorizationInvalidResponseException(response.Content);
        }
    }
}
