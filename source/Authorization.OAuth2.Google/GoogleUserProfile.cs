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

using Finebits.Authorization.OAuth2.Abstractions;

namespace Finebits.Authorization.OAuth2.Google
{
    public class GoogleUserProfile : IUserProfile, IUserAvatar
    {
        public string Id { get; protected internal set; }
        public string Email { get; protected internal set; }
        public string DisplayName { get; protected internal set; }
        public Uri Avatar { get; protected internal set; }

        public string Name { get; protected internal set; }
        public string FamilyName { get; protected internal set; }
        public bool IsEmailVerified { get; protected internal set; }
        public string Locale { get; protected internal set; }
    }
}
