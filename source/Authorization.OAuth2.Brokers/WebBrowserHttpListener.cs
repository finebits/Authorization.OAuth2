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
using System.Text;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Brokers.Abstractions;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Brokers
{
    public class WebBrowserHttpListener : AuthenticationListener, IDisposable
    {
        public string ResponseString { get; set; } = "<html><body>Please return to the application.</body></html>";

        private readonly HttpListener _httpListener;
        private bool _isDisposed;
        private bool _isCanceled;

        public WebBrowserHttpListener()
        {
            _httpListener = new HttpListener();
        }

        public override void Cancel()
        {
            _isCanceled = true;
            _httpListener.Abort();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected internal override async Task<AuthenticationResult> GetResultAsync()
        {
            try
            {
                var context = await _httpListener.GetContextAsync().ConfigureAwait(false);

                var response = context.Response;
                var buffer = Encoding.UTF8.GetBytes(ResponseString);
                response.ContentLength64 = buffer.Length;
                using (var responseOutput = response.OutputStream)
                {
                    responseOutput.Write(buffer, 0, buffer.Length);
                }
                _httpListener.Stop();

                return new AuthenticationResult(context?.Request?.QueryString ?? new NameValueCollection());
            }
            catch when (_isCanceled == true)
            {
                return AuthenticationResult.Canceled;
            }
            catch
            {
                throw;
            }
        }

        protected internal override void SetCallback(Uri callback)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _httpListener.Prefixes.Add(callback.ToString());
            _httpListener.Start();
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                // free managed resources
                _httpListener.Close();
            }

            _isDisposed = true;
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        ~WebBrowserHttpListener()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
    }
}
