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
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Google
{
    public partial class GoogleAuthClient : AuthorizationClient, IRefreshable, IRevocable, IProfileReader, IUserAvatarLoader
    {
        protected GoogleConfiguration Configuration => Config as GoogleConfiguration;

        public GoogleAuthClient(HttpClient httpClient, IAuthenticationBroker broker, GoogleConfiguration config)
            : base(httpClient, broker, config)
        { }

        protected override Task<Uri> GetAuthenticationEndpointAsync(string userId, object properties, CancellationToken cancellationToken)
        {
            Uri authorizationEndpoint = Configuration.AuthorizationUri;
            string clientId = Configuration.ClientId;
            string redirectUri = Uri.EscapeDataString(Configuration.RedirectUri.ToString());

            string endpoint = $"{authorizationEndpoint}?response_type=code&redirect_uri={redirectUri}&client_id={clientId}";

            if (properties is AuthProperties props)
            {
                endpoint += $"&state={props.State}&code_challenge={props.CodeChallenge}&code_challenge_method={props.CodeChallengeMethod}";
            }

            string scope = Configuration.GetScope();
            if (!string.IsNullOrEmpty(scope))
            {
                scope = Uri.EscapeDataString(scope);
                endpoint += $"&scope={scope}";
            }

            if (!string.IsNullOrEmpty(userId))
            {
                endpoint += $"&login_hint={userId}";
            }

            return Task.FromResult(new Uri(endpoint));
        }

        protected override async Task<AuthCredential> GetTokenAsync(AuthenticationResult result, object properties, CancellationToken cancellationToken)
        {
            GoogleTokenContent response = await SendRequestAsync<GoogleTokenContent>(
                endpoint: Configuration.TokenUri,
                method: HttpMethod.Post,
                credential: null,
                payload: GetTokenPayload(result, properties),
                headers: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return new GoogleAuthCredential(
                response.AccessToken,
                response.RefreshToken,
                response.TokenType,
                TimeSpan.FromSeconds(response.ExpiresIn),
                response.Scope,
                response.IdToken
                );
        }

        public Task<AuthCredential> RefreshAsync(Credential credential, CancellationToken cancellationToken = default)
        {
            return new RefreshableClient(this).RefreshAsync(credential, cancellationToken);
        }

        public Task RevokeAsync(Credential credential, CancellationToken cancellationToken = default)
        {
            return new RevocableClient(this)
            {
                RevokePayloadCreator = GetRevokePayload
            }.RevokeAsync(credential, cancellationToken);
        }

        protected static NameValueCollection GetRevokePayload(Credential credential)
        {
            return new RevokePayload()
            {
                RefreshToken = (credential ?? throw new ArgumentNullException(nameof(credential))).RefreshToken
            }.GetCollection();
        }

        public Task<IUserProfile> ReadProfileAsync(Credential credential, CancellationToken cancellationToken = default)
        {
            return new ProfileReader<GoogleProfileContent>(this)
            {
                UserProfileCreator = (content) => new GoogleUserProfile
                {
                    Id = content.Id,
                    Email = content.Email,
                    DisplayName = content.GivenName,
                    Avatar = Uri.TryCreate(content.Picture, UriKind.Absolute, out Uri avatar) ? avatar : null,
                    Name = content.Name,
                    FamilyName = content.FamilyName,
                    IsEmailVerified = content.IsEmailVerified,
                    Locale = content.Locale,
                }
            }.ReadProfileAsync(credential, cancellationToken);
        }

        public async Task<Stream> LoadAvatarAsync(Credential credential, CancellationToken cancellationToken = default)
        {
            if (credential is null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            cancellationToken.ThrowIfCancellationRequested();

            IUserProfile profile;

            try
            {
                profile = await ReadProfileAsync(credential, cancellationToken).ConfigureAwait(false);
            }
            catch (AuthorizationEmptyResponseException ex)
            {
                throw new AuthorizationDownloadFileException(AuthorizationDownloadFileException.DefaultMessage, ex);
            }

            if (profile is GoogleUserProfile googleProfile && googleProfile.Avatar is not null)
            {
                return await DownloadFileAsync<EmptyContent>(
                    endpoint: googleProfile.Avatar,
                    method: HttpMethod.Get,
                    credential: null,
                    headers: null,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            throw new AuthorizationDownloadFileException();
        }
    }
}
