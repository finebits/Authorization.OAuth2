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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.Messages;
using Finebits.Authorization.OAuth2.Types;
using Finebits.Network.RestClient;

namespace Finebits.Authorization.OAuth2
{
    public abstract partial class AuthorizationClient
    {
        protected static string AuthCodePropertyName { get; } = "code";
        protected static string AuthStatePropertyName { get; } = "state";

        protected Task<TContent> SendRequestAsync<TContent>(
            Uri endpoint,
            HttpMethod method,
            Token token,
            NameValueCollection payload,
            HeaderCollection headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            var client = new NetworkClient(HttpClient);
            return client.SendRequestAsync<TContent>(endpoint, method, payload, TryAddAuthorizationHeader(headers, token), cancellationToken);
        }

        protected Task<TContent> SendRequestAsync<TContent>(
            Uri endpoint,
            HttpMethod method,
            Token token,
            HeaderCollection headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            var client = new NetworkClient(HttpClient);
            return client.SendRequestAsync<TContent>(endpoint, method, TryAddAuthorizationHeader(headers, token), cancellationToken);
        }

        protected Task<Stream> SendRequestAsync(
            Uri endpoint,
            HttpMethod method,
            Token token,
            HeaderCollection headers,
            CancellationToken cancellationToken)
        {
            var client = new NetworkClient(HttpClient);
            return client.SendRequestAsync(endpoint, method, TryAddAuthorizationHeader(headers, token), cancellationToken);
        }

        protected static (string method, string verifier, string challenge) GenerateCodeChallengeSHA256()
        {
            const string method = "S256";

            var verifier = Tools.Cryptography.GenerateRandomString(64);

            var buffer = Encoding.UTF8.GetBytes(verifier);
            var hash = Tools.Cryptography.GetHashSha256(buffer);
            string challenge = Tools.Cryptography.ConvertToBase64UrlEncode(hash, true);

            return (method, verifier, challenge);
        }

        protected static string GenerateState()
        {
            return Tools.Cryptography.GenerateRandomString(32);
        }

        protected static string GetAuthCode(AuthenticationResult result) => GetPropertyValue(result?.Properties, AuthCodePropertyName);

        protected static bool IsStateCorrect(AuthenticationResult result, string state)
        {
            return state?.Equals(GetPropertyValue(result?.Properties, AuthStatePropertyName), StringComparison.Ordinal) is true;
        }

        protected static string GetPropertyValue(NameValueCollection nameValueCollection, string propertyName)
        {
            var properties = nameValueCollection ?? throw new AuthorizationPropertiesException();
            return properties[propertyName] ?? throw new AuthorizationPropertiesException(null, propertyName);
        }

        private static HeaderCollection TryAddAuthorizationHeader(HeaderCollection headers, Token token)
        {
            if (token is null)
            {
                return headers;
            }

            var result = new HeaderCollection(headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>());
            var authorizationHeader = new HeaderCollection(new[]
            {
                ("Authorization", new System.Net.Http.Headers.AuthenticationHeaderValue(token.TokenType, token.AccessToken).ToString())
            });

            return new HeaderCollection(result.Union(authorizationHeader));
        }
    }
}
