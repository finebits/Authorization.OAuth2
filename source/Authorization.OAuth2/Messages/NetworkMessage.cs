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
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

using Finebits.Authorization.OAuth2.RestClient;
using Finebits.Network.RestClient;

namespace Finebits.Authorization.OAuth2.Messages
{
    internal class NetworkMessage<TContent>
        : CommonMessage<JsonResponse<TContent>, FormUrlEncodedRequest<IFormUrlEncodedPayload>>
    {
        public override Uri Endpoint { get; }
        public override HttpMethod Method => HttpMethod.Post;

        private readonly IFormUrlEncodedPayload _payload;
        private readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> _headers;

        public NetworkMessage(
            Uri endpoint,
            IFormUrlEncodedPayload payload,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            _payload = payload;
            _headers = headers;
            Endpoint = endpoint;
        }

        protected override FormUrlEncodedRequest<IFormUrlEncodedPayload> CreateRequest()
        {
            HeaderCollection headers = null;

            if (_headers != null)
            {
                headers = new HeaderCollection
                (
                    headers: _headers,
                    headerValidation: false
                );
            }

            return new FormUrlEncodedRequest<IFormUrlEncodedPayload>
            {
                Payload = _payload,
                Headers = headers
            };
        }

        protected override JsonResponse<TContent> CreateResponse()
        {
            return new JsonResponse<TContent>
            {
                Options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                }
            };
        }
    }

    internal class EmptyNetworkMessage<TContent>
        : CommonMessage<JsonResponse<TContent>, EmptyRequest>
    {
        public override Uri Endpoint { get; }
        public override HttpMethod Method => HttpMethod.Post;

        private readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> _headers;

        public EmptyNetworkMessage(
            Uri endpoint,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            _headers = headers;
            Endpoint = endpoint;
        }

        protected override EmptyRequest CreateRequest()
        {
            HeaderCollection headers = null;

            if (_headers != null)
            {
                headers = new HeaderCollection
                (
                    headers: _headers,
                    headerValidation: false
                );
            }

            return new EmptyRequest
            {
                Headers = headers
            };
        }

        protected override JsonResponse<TContent> CreateResponse()
        {
            return new JsonResponse<TContent>
            {
                Options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                }
            };
        }
    }
}
