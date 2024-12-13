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
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2
{
    public abstract partial class AuthorizationClient : IAuthorizationClient
    {
        protected IAuthenticationBroker Broker { get; }
        protected AuthConfiguration Config { get; }
        protected HttpClient HttpClient { get; }

        protected AuthorizationClient(HttpClient httpClient, IAuthenticationBroker broker, AuthConfiguration config)
        {
            if (httpClient is null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (broker is null)
            {
                throw new ArgumentNullException(nameof(broker));
            }

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            HttpClient = httpClient;
            Broker = broker;
            Config = (AuthConfiguration)config.Clone();
        }

        public Task<AuthCredential> LoginAsync(CancellationToken cancellationToken = default)
        {
            return StartLoginAsync(null, cancellationToken);
        }

        public Task<AuthCredential> LoginAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Value cannot be empty.", nameof(userId));
            }

            return StartLoginAsync(userId, cancellationToken);
        }

        public virtual async Task<AuthCredential> StartLoginAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            object properties = await PrepareAsync(userId, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            Uri requestUri = await GetAuthenticationEndpointAsync(userId, properties, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            Uri callbackUri = await GetCallbackUriAsync(properties, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            AuthenticationResult result = await AuthenticateAsync(requestUri, callbackUri, cancellationToken).ConfigureAwait(false);

            ThrowIfAuthenticationUnsuccessful(result);

            cancellationToken.ThrowIfCancellationRequested();
            return await AuthorizeAsync(result, properties, cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task<object> PrepareAsync(string userId, CancellationToken cancellationToken)
        {
            string state = GenerateState();
            (string method, string verifier, string challenge) = GenerateCodeChallengeSHA256();

            return Task.FromResult<object>(new AuthProperties()
            {
                State = state,
                CodeChallenge = challenge,
                CodeChallengeMethod = method,
                CodeVerifier = verifier,
            });
        }

        protected virtual Task<Uri> GetCallbackUriAsync(object properties, CancellationToken cancellationToken)
        {
            return Task.FromResult(Config.RedirectUri);
        }

        protected virtual async Task<AuthCredential> AuthorizeAsync(AuthenticationResult result, object properties, CancellationToken cancellationToken)
        {
            AuthContent response = await SendRequestAsync<AuthContent>(
                endpoint: Config.TokenUri,
                method: HttpMethod.Post,
                credential: null,
                payload: GetTokenPayload(result, properties),
                headers: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return new AuthCredential(
                response.AccessToken,
                response.RefreshToken,
                response.TokenType,
                TimeSpan.FromSeconds(response.ExpiresIn),
                response.Scope
                );
        }

        protected virtual void ThrowIfAuthenticationUnsuccessful(AuthenticationResult result)
        {
            if (result is null)
            {
                throw new AuthorizationEmptyResponseException("The result of the authentication operation is empty.", new ArgumentNullException(nameof(result)));
            }

            NameValueCollection properties = result.Properties;

            string error = properties?.Get("error");
            if (!string.IsNullOrEmpty(error))
            {
                throw new AuthorizationBrokerResultException(
                    properties: properties,
                    message: "Authorization cannot be done. The result of the authentication operation contains an error.",
                    innerException: null);
            }

            if (properties is null || properties.Count == 0)
            {
                throw new AuthorizationPropertiesException();
            }
        }

        protected abstract Task<Uri> GetAuthenticationEndpointAsync(string userId, object properties, CancellationToken cancellationToken);

        protected virtual NameValueCollection GetTokenPayload(AuthenticationResult result, object properties)
        {
            if (properties is AuthProperties props)
            {
                if (!IsStateCorrect(result, props.State))
                {
                    throw new AuthorizationPropertiesException("A property has an unexpected value.", AuthStatePropertyName);
                }

                string code = GetAuthCode(result);

                return new AuthPayload()
                {
                    ClientId = Config.ClientId,
                    ClientSecret = Config.ClientSecret,
                    Code = code,
                    CodeVerifier = props.CodeVerifier,
                    RedirectUri = Config.RedirectUri,
                }.GetCollection();
            }

            throw new ArgumentException("The argument has an unexpected type.", nameof(properties));
        }

        private async Task<AuthenticationResult> AuthenticateAsync(Uri requestUri, Uri callbackUri, CancellationToken cancellationToken)
        {
            try
            {
                return await Broker.AuthenticateAsync(requestUri, callbackUri, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AuthorizationException("Authentication operation failed.", ex);
            }
        }
    }
}
