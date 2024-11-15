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
            using (NetworkMessage<TContent> message = new NetworkMessage<TContent>(endpoint, method, payload, headers))
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

        internal async Task<TContent> SendEmptyRequestAsync<TContent>(
            Uri endpoint,
            HttpMethod method,
            HeaderCollection headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            using (EmptyNetworkMessage<TContent> message = new EmptyNetworkMessage<TContent>(endpoint, method, headers))
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

        internal async Task<Stream> DownloadFileAsync<TError>(
            Uri endpoint,
            HttpMethod method,
            HeaderCollection headers,
            CancellationToken cancellationToken)
        {
            using (StreamNetworkMessage<TError> message = new StreamNetworkMessage<TError>(endpoint, method, headers))
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

                    if (message.Response.PickedResponse is StreamNetworkMessage<TError>.ErrorResponse error &&
                        error.Content is IInvalidResponse invalidResponse)
                    {
                        content = invalidResponse;
                    }

                    throw new AuthorizationInvalidResponseException(content, ex);
                }

                if (message.Response.PickedResponse is StreamResponse streamResponse && streamResponse.Stream?.Length > 0)
                {
                    MemoryStream result = new MemoryStream();
                    streamResponse.Stream?.CopyTo(result);
                    result.Position = 0;
                    return result;
                }

                throw new AuthorizationDownloadFileException();
            }
        }

        private static T GetContent<T>(Network.RestClient.JsonResponse<T> response)
            where T : IInvalidResponse
        {
            if (response is null || (response.Content == null && typeof(T) != typeof(AuthorizationClient.EmptyContent)))
            {
                throw new AuthorizationEmptyResponseException();
            }

            return string.IsNullOrEmpty(response.Content?.ErrorReason)
                ? (response.Content is ICloneable cloneable) ? (T)cloneable.Clone() : response.Content
                : throw new AuthorizationInvalidResponseException(response.Content);
        }
    }
}
