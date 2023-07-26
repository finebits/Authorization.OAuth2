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
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Brokers.Abstractions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Brokers
{
    public class WebBrowserAuthenticationBroker : IAuthenticationBroker
    {
        private readonly IWebBrowserLauncher _launcher;
        private readonly AuthenticationListener _listener;

        public WebBrowserAuthenticationBroker(IWebBrowserLauncher launcher, AuthenticationListener listener)
        {
            if (launcher is null)
            {
                throw new ArgumentNullException(nameof(launcher));
            }

            if (listener is null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            _launcher = launcher;
            _listener = listener;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(Uri requestUri, Uri callbackUri)
        {
            if (requestUri is null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (callbackUri is null)
            {
                throw new ArgumentNullException(nameof(callbackUri));
            }

            _listener.SetCallback(callbackUri);

            if (await _launcher.LaunchAsync(requestUri).ConfigureAwait(false))
            {
                return await _listener.GetResultAsync().ConfigureAwait(false);
            }

            throw new InvalidOperationException($"{nameof(IWebBrowserLauncher)} can't be launched.");
        }
    }
}
