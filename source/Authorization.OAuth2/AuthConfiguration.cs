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

namespace Finebits.Authorization.OAuth2
{
    public class AuthConfiguration : ICloneable
    {
        public Uri AuthorizationUri { get; protected set; }
        public Uri TokenUri { get; protected set; }
        public Uri RefreshUri { get; protected set; }
        public Uri RevokeUri { get; protected set; }
        public Uri UserProfileUri { get; protected set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public Uri RedirectUri { get; set; }

        public IReadOnlyCollection<string> ScopeList { get; set; }

        public AuthConfiguration(Uri authorizationEndpoint, Uri tokenEndpoint, Uri refreshEndpoint, Uri revokeEndpoint, Uri userProfileEndpoint)
        {
            AuthorizationUri = authorizationEndpoint;
            TokenUri = tokenEndpoint;
            RefreshUri = refreshEndpoint;
            RevokeUri = revokeEndpoint;
            UserProfileUri = userProfileEndpoint;
        }

        public string GetScope()
        {
            return string.Join(" ", ScopeList);
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
