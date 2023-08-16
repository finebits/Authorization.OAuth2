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
using System.Diagnostics;
using System.Threading.Tasks;

using Finebits.Authorization.OAuth2.Brokers.Abstractions;

namespace Finebits.Authorization.OAuth2.Brokers
{
    public class WebBrowserLauncher : IWebBrowserLauncher
    {
        public Task<bool> LaunchAsync(Uri uri)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.IsAbsoluteUri && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                ? Task.FromResult(StartProcess(uri.ToString()))
                : Task.FromResult(false);
        }

        protected static bool StartProcess(string fileName)
        {
            using (var process = Process.Start(new ProcessStartInfo { FileName = fileName, UseShellExecute = true }))
            {
                return process != null;
            }
        }
    }
}
