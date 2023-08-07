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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Network.RestClient;

//ToDo: Move to RestClient
namespace Finebits.Authorization.OAuth2.RestClient
{
    public interface IFormUrlEncodedPayload
    {
        NameValueCollection GetCollection();
    }

    internal class FormUrlEncodedRequest<TFormUrlEncodedPayload> : Request
        where TFormUrlEncodedPayload : IFormUrlEncodedPayload
    {
        public IFormUrlEncodedPayload Payload { get; set; }

        protected override Task<HttpContent> CreateContentAsync(CancellationToken cancellationToken)
        {
            var collection = Payload.GetCollection();
            return Task.FromResult<HttpContent>(new FormUrlEncodedContent(collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection[key]))));
        }
    }
}
