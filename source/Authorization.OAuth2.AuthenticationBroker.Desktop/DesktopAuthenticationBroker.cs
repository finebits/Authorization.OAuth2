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
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.AuthenticationBroker.Abstractions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.AuthenticationBroker
{
    public class DesktopAuthenticationBroker : IAuthenticationBroker
    {
        public const int WrongPort = int.MinValue;
        public string ResponseString { get; set; } = "<html><body>Please return to the application.</body></html>";
        private readonly IWebBrowserLauncher _launcher;

        public static bool IsSupported => HttpListener.IsSupported;

        public DesktopAuthenticationBroker(IWebBrowserLauncher launcher)
        {
            if (launcher is null)
            {
                throw new ArgumentNullException(nameof(launcher));
            }

            _launcher = launcher;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(Uri requestUri, Uri callbackUri, CancellationToken cancellationToken = default)
        {
            if (requestUri is null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (callbackUri is null)
            {
                throw new ArgumentNullException(nameof(callbackUri));
            }

            cancellationToken.ThrowIfCancellationRequested();

            using (var httpListener = new HttpListener())
            {
                const int ERROR_OPERATION_ABORTED = 995;
                try
                {
                    httpListener.Prefixes.Add(callbackUri.ToString());
                    httpListener.Start();

                    if (!await _launcher.LaunchAsync(requestUri).ConfigureAwait(false))
                    {
                        throw new InvalidOperationException($"The web browser cannot be launched.");
                    }

                    using (var ctr = cancellationToken.Register(() => httpListener.Stop()))
                    {
                        var context = await httpListener.GetContextAsync().ConfigureAwait(false);

                        var response = context.Response;
                        var buffer = Encoding.UTF8.GetBytes(ResponseString);
                        response.ContentLength64 = buffer.Length;
                        using (var responseOutput = response.OutputStream)
                        {
                            responseOutput.Write(buffer, 0, buffer.Length);
                        }

                        return new AuthenticationResult(context?.Request?.QueryString ?? new NameValueCollection());
                    }
                }
                catch (HttpListenerException exHttpListener) when (exHttpListener.ErrorCode == ERROR_OPERATION_ABORTED &&
                                                                   cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancellationToken);
                }
                catch (InvalidOperationException) when (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancellationToken);
                }
                finally
                {
                    httpListener.Stop();
                    httpListener.Close();
                }
            }
        }

        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, IPEndPoint.MinPort);
            try
            {
                listener.Start();

                return listener.LocalEndpoint is IPEndPoint ipEndPoint ? ipEndPoint.Port : throw new InvalidOperationException("Could not find an available local port.");
            }
            finally
            {
                listener.Stop();
            }
        }

        public static Uri GetLoopbackUri()
        {
            return new Uri($"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/");
        }
    }
}
