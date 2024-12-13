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
            Credential credential,
            NameValueCollection payload,
            HeaderCollection headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            NetworkClient client = new(HttpClient);
            return client.SendRequestAsync<TContent>(endpoint, method, payload, TryAddAuthorizationHeader(headers, credential), cancellationToken);
        }

        protected Task<TContent> SendEmptyRequestAsync<TContent>(
            Uri endpoint,
            HttpMethod method,
            Credential credential,
            HeaderCollection headers,
            CancellationToken cancellationToken)
            where TContent : IInvalidResponse
        {
            NetworkClient client = new(HttpClient);
            return client.SendEmptyRequestAsync<TContent>(endpoint, method, TryAddAuthorizationHeader(headers, credential), cancellationToken);
        }

        protected Task<Stream> DownloadFileAsync<TError>(
            Uri endpoint,
            HttpMethod method,
            Credential credential,
            HeaderCollection headers,
            CancellationToken cancellationToken)
        {
            NetworkClient client = new(HttpClient);
            return client.DownloadFileAsync<TError>(endpoint, method, TryAddAuthorizationHeader(headers, credential), cancellationToken);
        }

        protected static (string method, string verifier, string challenge) GenerateCodeChallengeSHA256()
        {
            const string Method = "S256";

            string verifier = Tools.Cryptography.GenerateRandomString(64);

            byte[] buffer = Encoding.UTF8.GetBytes(verifier);
            byte[] hash = Tools.Cryptography.GetHashSha256(buffer);
            string challenge = Tools.Cryptography.ConvertToBase64UrlEncode(hash, true);

            return (Method, verifier, challenge);
        }

        protected static string GenerateState()
        {
            return Tools.Cryptography.GenerateRandomString(32);
        }

        protected static string GetAuthCode(AuthenticationResult result)
        {
            return GetPropertyValue(result?.Properties, AuthCodePropertyName);
        }

        protected static bool IsStateCorrect(AuthenticationResult result, string state)
        {
            return state?.Equals(GetPropertyValue(result?.Properties, AuthStatePropertyName), StringComparison.Ordinal) is true;
        }

        protected static string GetPropertyValue(NameValueCollection nameValueCollection, string propertyName)
        {
            NameValueCollection properties = nameValueCollection ?? throw new AuthorizationPropertiesException();
            return properties[propertyName] ?? throw new AuthorizationPropertiesException(null, propertyName);
        }

        private static HeaderCollection TryAddAuthorizationHeader(HeaderCollection headers, Credential credential)
        {
            if (credential is null)
            {
                return headers;
            }

            HeaderCollection result = new(headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>());
            HeaderCollection authorizationHeader = new(
            [
                ("Authorization", new System.Net.Http.Headers.AuthenticationHeaderValue(credential.TokenType, credential.AccessToken).ToString())
            ]);

            return new HeaderCollection(result.Union(authorizationHeader));
        }
    }
}
