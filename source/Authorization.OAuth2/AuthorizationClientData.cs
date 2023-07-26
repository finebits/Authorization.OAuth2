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

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.RestClient;

namespace Finebits.Authorization.OAuth2
{
    public abstract partial class AuthorizationClient : AuthorizationRefreshableClient, IRefreshable, IRevocable
    {
        protected class RevokePayload : IFormUrlEncodedPayload
        {
            public string RefreshToken { get; set; }

            public NameValueCollection GetCollection()
            {
                return new NameValueCollection
                {
                    {"token", RefreshToken}
                };
            }
        }
    }
}
