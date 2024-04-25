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
    public class Token
    {
        public const string BearerType = "Bearer";
        public const string DefaultTokenType = BearerType;

        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string TokenType { get; private set; }

        public Token(string accessToken, string refreshToken, string tokenType)
        {
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
            TokenType = tokenType ?? DefaultTokenType;
        }

        public Token(Token other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            AccessToken = other.AccessToken;
            RefreshToken = other.RefreshToken;
            TokenType = other.TokenType;
        }

        public virtual void Update(Token other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            AccessToken = GetValueOrDefault(other.AccessToken, AccessToken);
            RefreshToken = GetValueOrDefault(other.RefreshToken, RefreshToken);
            TokenType = GetValueOrDefault(other.TokenType, TokenType);
        }

        protected static string GetValueOrDefault(string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
    }
}
