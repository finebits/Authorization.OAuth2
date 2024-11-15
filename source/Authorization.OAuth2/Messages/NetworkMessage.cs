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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

using Finebits.Network.RestClient;

namespace Finebits.Authorization.OAuth2.Messages
{
    internal class NetworkMessage<TContent>
        : CommonMessage<ForceJsonResponse<TContent>, FormUrlEncodedRequest>
    {
        public override Uri Endpoint { get; }
        public override HttpMethod Method { get; }

        private readonly NameValueCollection _payload;
        private readonly HeaderCollection _headers;

        public NetworkMessage(
            Uri endpoint,
            HttpMethod method,
            NameValueCollection payload,
            HeaderCollection headers)
        {
            _payload = payload ?? [];
            _headers = headers;
            Method = method;
            Endpoint = endpoint;
        }

        protected override FormUrlEncodedRequest CreateRequest()
        {
            HeaderCollection headers = null;

            if (_headers is not null)
            {
                headers = new HeaderCollection
                (
                    headers: _headers,
                    headerValidation: false
                );
            }

            return new FormUrlEncodedRequest(_payload.AllKeys.Select(key => new KeyValuePair<string, string>(key, _payload[key])))
            {
                Headers = headers
            };
        }

        protected override ForceJsonResponse<TContent> CreateResponse()
        {
            return new ForceJsonResponse<TContent>
            {
                Options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                }
            };
        }
    }

    internal class EmptyNetworkMessage<TContent>
        : CommonMessage<ForceJsonResponse<TContent>, EmptyRequest>
    {
        public override Uri Endpoint { get; }
        public override HttpMethod Method { get; }

        private readonly HeaderCollection _headers;

        public EmptyNetworkMessage(
            Uri endpoint,
            HttpMethod method,
            HeaderCollection headers)
        {
            _headers = headers;
            Method = method;
            Endpoint = endpoint;
        }

        protected override EmptyRequest CreateRequest()
        {
            HeaderCollection headers = null;

            if (_headers is not null)
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

        protected override ForceJsonResponse<TContent> CreateResponse()
        {
            return new ForceJsonResponse<TContent>
            {
                Options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                }
            };
        }
    }

    internal class StreamNetworkMessage<TError>
        : CommonMessage<FlexibleResponse, EmptyRequest>
    {
        public class ErrorResponse : ForceJsonResponse<TError> { }
        public override Uri Endpoint { get; }
        public override HttpMethod Method { get; }

        private readonly HeaderCollection _headers;

        public StreamNetworkMessage(
            Uri endpoint,
            HttpMethod method,
            HeaderCollection headers)
        {
            _headers = headers;
            Method = method;
            Endpoint = endpoint;
        }

        protected override EmptyRequest CreateRequest()
        {
            HeaderCollection headers = null;

            if (_headers is not null)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "StreamResponse is disposed by FlexibleResponse")]
        protected override FlexibleResponse CreateResponse()
        {
            StreamResponse streamResponse = new();

            try
            {
                return new FlexibleResponse(
                            [
                                new ErrorResponse()
                                {
                                    Options = new JsonSerializerOptions
                                    {
                                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                    }
                                },
                                streamResponse
                            ]);
            }
            catch
            {
                streamResponse.Dispose();
                throw;
            }
        }
    }
}
