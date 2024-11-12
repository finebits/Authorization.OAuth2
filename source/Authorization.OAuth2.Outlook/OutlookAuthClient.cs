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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Outlook
{
    public partial class OutlookAuthClient : AuthorizationClient, IRefreshable
    {
        protected OutlookConfiguration Configuration => Config as OutlookConfiguration;

        public OutlookAuthClient(HttpClient httpClient, IAuthenticationBroker broker, OutlookConfiguration config)
            : base(httpClient, broker, config)
        { }

        protected override Task<Uri> GetAuthenticationEndpointAsync(string userId, object properties, CancellationToken cancellationToken)
        {
            var authorizationEndpoint = Configuration.AuthorizationUri;
            var clientId = Configuration.ClientId;
            var redirectUri = Uri.EscapeDataString(Configuration.RedirectUri.ToString());

            var endpoint = $"{authorizationEndpoint}?response_type=code&response_mode=query&redirect_uri={redirectUri}&client_id={clientId}";

            if (properties is AuthProperties props)
            {
                endpoint += $"&state={props.State}&code_challenge={props.CodeChallenge}&code_challenge_method={props.CodeChallengeMethod}";
            }

            var scope = Configuration.GetScope();
            if (!string.IsNullOrEmpty(scope))
            {
                scope = Uri.EscapeDataString(scope);
                endpoint += $"&scope={scope}";
            }

            if (!string.IsNullOrEmpty(userId))
            {
                endpoint += $"&login_hint={userId}";
            }

            if (Configuration.Prompt != OutlookAuthPrompt.None)
            {
                endpoint += $"&prompt={OutlookConfiguration.ConvertPromptToString(Configuration.Prompt)}";
            }

            return Task.FromResult(new Uri(endpoint));
        }

        public Task<AuthorizationToken> RefreshTokenAsync(Token token, CancellationToken cancellationToken = default)
        {
            return new RefreshableClient(this).RefreshTokenAsync(token, cancellationToken);
        }

        public Task<IUserProfile> ReadProfileAsync(Token token, CancellationToken cancellationToken = default)
        {
            return new ProfileReader<OutlookProfileContent>(this)
            {
                UserProfileCreator = (content) => new OutlookUserProfile
                {
                    Id = content.Id,
                    Email = content.Mail,
                    DisplayName = content.DisplayName,
                    Alias = content.Alias,
                    MailboxGuid = content.MailboxGuid,
                }
            }.ReadProfileAsync(token, cancellationToken);
        }
    }
}
