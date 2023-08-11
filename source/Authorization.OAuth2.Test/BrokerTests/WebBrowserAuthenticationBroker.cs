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

using System.Diagnostics.CodeAnalysis;
using System.Net;

using Finebits.Authorization.OAuth2.Brokers;
using Finebits.Authorization.OAuth2.Brokers.Abstractions;

using Moq;

namespace Finebits.Authorization.OAuth2.Test.BrokerTests;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class WebBrowserAuthenticationBrokerTests
{
    [Test]
    public void Constructor_NullParam_Exception()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new WebBrowserAuthenticationBroker(null));
        Assert.That(exception.ParamName, Is.EqualTo("launcher"));
    }

    [Test]
    [TestCase(null, null, "requestUri")]
    [TestCase(null, "https://callback", "requestUri")]
    [TestCase("https://request", null, "callbackUri")]
    public void AuthenticateAsync_NullParam_Exception(string requestStringUri, string callbackStringUri, string paramName)
    {
        var broker = new WebBrowserAuthenticationBroker(new Mock<IWebBrowserLauncher>().Object);

        Uri? requestUri = string.IsNullOrEmpty(requestStringUri) ? null : new(requestStringUri);
        Uri? callbackUri = string.IsNullOrEmpty(callbackStringUri) ? null : new(callbackStringUri);

        var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await broker.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo(paramName));
    }

    [Test]
    public void AuthenticateAsync_NonLaunched_Exception()
    {
        var mockLauncher = new Mock<IWebBrowserLauncher>();
        mockLauncher.Setup(m => m.LaunchAsync(It.IsAny<Uri>())).Returns(Task.FromResult(false));

        var broker = new WebBrowserAuthenticationBroker(mockLauncher.Object);
        var callback = WebBrowserAuthenticationBroker.GetLoopbackUri();

        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await broker.AuthenticateAsync(new Uri("https://request/"), callback).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void GetRandomUnusedPort_Call_Success()
    {
        var port = int.MinValue;

        Assert.DoesNotThrow(() => port = WebBrowserAuthenticationBroker.GetRandomUnusedPort());

        Assert.That(port, Is.InRange(IPEndPoint.MinPort, IPEndPoint.MaxPort));
    }

    [Test]
    public void GetDefaultCallbackUri_Call_Success()
    {
        Uri? uri = null;

        Assert.DoesNotThrow(() => uri = WebBrowserAuthenticationBroker.GetLoopbackUri());

        Assert.That(uri, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(uri.IsAbsoluteUri, Is.True);
            Assert.That(uri.IsLoopback, Is.True);
            Assert.That(uri.Port, Is.InRange(IPEndPoint.MinPort, IPEndPoint.MaxPort));
        });
    }
}
