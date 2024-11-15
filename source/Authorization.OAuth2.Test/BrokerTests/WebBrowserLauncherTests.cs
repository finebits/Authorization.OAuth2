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

using System.Diagnostics.CodeAnalysis;

using Finebits.Authorization.OAuth2.AuthenticationBroker;

namespace Finebits.Authorization.OAuth2.Test.BrokerTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Unit Test Naming Conventions")]
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class WebBrowserLauncherTests
{
    [Test]
    public void LaunchAsync_NullUri_Exception()
    {
        WebBrowserLauncher webBrowserLauncher = new();

        ArgumentNullException? exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await webBrowserLauncher.LaunchAsync(null).ConfigureAwait(false));

        Assert.That(exception.ParamName, Is.EqualTo("uri"));
    }

    [Test]
    [TestCase("file://any")]
    [TestCase("custom://any")]
    [TestCase("custom:\\any")]
    public async Task LaunchAsync_UnsupportedUri_Fail(Uri uri)
    {
        WebBrowserLauncher webBrowserLauncher = new();

        bool result = await webBrowserLauncher.LaunchAsync(uri).ConfigureAwait(false);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task LaunchAsync_RelativeUri_Fail()
    {
        Uri uri = new("test/test", UriKind.Relative);

        WebBrowserLauncher webBrowserLauncher = new();

        bool result = await webBrowserLauncher.LaunchAsync(uri).ConfigureAwait(false);
        Assert.That(result, Is.False);
    }
}
