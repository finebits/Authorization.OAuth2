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

namespace Finebits.Authorization.OAuth2.Types
{
    public class AuthorizationToken : Token
    {
        public TimeSpan ExpiresIn { get; private set; }
        public string Scope { get; private set; }

        public AuthorizationToken(string accessToken, string refreshToken, string tokenType, TimeSpan expiresIn, string scope)
            : base(accessToken, refreshToken, tokenType)
        {
            ExpiresIn = expiresIn;
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public AuthorizationToken(AuthorizationToken other)
            : base(other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            ExpiresIn = other.ExpiresIn;
            Scope = other.Scope;
        }

        public override void Update(Token other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            base.Update(other);

            if (other is AuthorizationToken token)
            {
                ExpiresIn = token.ExpiresIn;
                Scope = GetValueOrDefault(token.Scope, Scope);
            }
        }
    }
}
