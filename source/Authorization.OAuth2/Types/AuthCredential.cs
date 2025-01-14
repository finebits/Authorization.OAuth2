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
    public class AuthCredential : Credential
    {
        public TimeSpan ExpiresIn { get; private set; }
        public string IdToken { get; private set; }
        public string Scope { get; private set; }

        public AuthCredential(string tokenType, string accessToken, string refreshToken, string idToken, TimeSpan expiresIn, string scope)
            : base(tokenType, accessToken, refreshToken)
        {
            IdToken = idToken ?? throw new ArgumentNullException(nameof(idToken));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ExpiresIn = expiresIn;
        }

        public AuthCredential(AuthCredential other)
            : base(other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            ExpiresIn = other.ExpiresIn;
            Scope = other.Scope;
        }

        public override void Update(Credential other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            base.Update(other);

            if (other is AuthCredential credential)
            {
                ExpiresIn = credential.ExpiresIn;
                IdToken = GetValueOrDefault(credential.IdToken, IdToken);
                Scope = GetValueOrDefault(credential.Scope, Scope);
            }
        }
    }
}
