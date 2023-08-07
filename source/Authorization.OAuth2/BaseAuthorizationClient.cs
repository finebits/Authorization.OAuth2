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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.RestClient;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2
{
    public abstract partial class BaseAuthorizationClient : IAuthorizationClient
    {
        protected IAuthenticationBroker Broker { get; }
        protected AuthConfiguration Config { get; }
        protected HttpClient HttpClient { get; }

        protected BaseAuthorizationClient(HttpClient httpClient, IAuthenticationBroker broker, AuthConfiguration config)
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

        public Task<AuthorizationToken> LoginAsync(CancellationToken cancellationToken = default)
        {
            return StartLoginAsync(null, cancellationToken);
        }

        public Task<AuthorizationToken> LoginAsync(string userId, CancellationToken cancellationToken = default)
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

        public virtual async Task<AuthorizationToken> StartLoginAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var properties = await PrepareAsync(userId, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            var requestUri = await GetAuthenticationEndpointAsync(userId, properties, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            var callbackUri = await GetCallbackUriAsync(properties, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            var result = await AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

            ThrowIfBrokerError(result);

            cancellationToken.ThrowIfCancellationRequested();
            return await GetTokenAsync(result, properties, cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task<object> PrepareAsync(string userId, CancellationToken cancellationToken)
        {
            var state = GenerateState();
            var (method, verifier, challenge) = GenerateCodeChallengeSHA256();

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

        protected virtual async Task<AuthorizationToken> GetTokenAsync(AuthenticationResult result, object properties, CancellationToken cancellationToken)
        {
            var response = await SendRequestAsync<TokenContent>(
                endpoint: Config.TokenUri,
                payload: GetTokenPayload(result, properties),
                headers: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return new AuthorizationToken(
                response.AccessToken,
                response.RefreshToken,
                response.TokenType,
                TimeSpan.FromSeconds(response.ExpiresIn),
                response.Scope
                );
        }

        protected virtual void ThrowIfBrokerError(AuthenticationResult result)
        {
            if (result is null)
            {
                throw new AuthorizationEmptyResponseException("IAuthenticationBroker doesn't return a result.", new ArgumentNullException(nameof(result)));
            }

            if (result == AuthenticationResult.Canceled)
            {
                throw new AuthorizationException(ErrorType.Cancel);
            }

            var properties = result.Properties;

            var error = properties?.Get("error");
            if (!string.IsNullOrEmpty(error))
            {
                throw new AuthorizationInvalidBrokerResultException(
                    properties: properties,
                    message: "Authorization cannot be done. IAuthenticationBroker result contains an error.",
                    innerException: null);
            }

            if (properties is null || properties.Count == 0)
            {
                throw new AuthorizationPropertiesException();
            }
        }

        protected abstract Task<Uri> GetAuthenticationEndpointAsync(string userId, object properties, CancellationToken cancellationToken);

        protected virtual IFormUrlEncodedPayload GetTokenPayload(AuthenticationResult result, object properties)
        {
            if (properties is AuthProperties props)
            {
                if (!IsStateCorrect(result, props.State))
                {
                    throw new AuthorizationPropertiesException("A property has an unexpected value.", AuthStatePropertyName);
                }

                var code = GetAuthCode(result);

                return new TokenPayload()
                {
                    ClientId = Config.ClientId,
                    ClientSecret = Config.ClientSecret,
                    Code = code,
                    CodeVerifier = props.CodeVerifier,
                    RedirectUri = Config.RedirectUri,
                };
            }

            throw new ArgumentException("The argument has an unexpected type.", nameof(properties));
        }

        private Task<AuthenticationResult> AuthenticateAsync(Uri requestUri, Uri callbackUri)
        {
            try
            {
                return Broker.AuthenticateAsync(requestUri, callbackUri);
            }
            catch (Exception ex)
            {
                throw new AuthorizationException("IAuthenticationBroker.AuthenticateAsync operation failed", ex);
            }
        }
    }
}
