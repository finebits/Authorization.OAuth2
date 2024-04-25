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

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Network.RestClient;

namespace Finebits.Authorization.OAuth2.Messages
{
    public class ForceJsonResponse<TContent> : JsonResponse<TContent>
    {
        protected override async Task<bool> ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content == null)
            {
                return false;
            }

            return await TryReadJson(content, cancellationToken).ConfigureAwait(false);
        }

        private async Task<bool> TryReadJson(HttpContent content, CancellationToken cancellationToken)
        {
            try
            {
                Content = await content.ReadFromJsonAsync<TContent>(Options, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (JsonException)
            { }

            return false;
        }
    }
}
