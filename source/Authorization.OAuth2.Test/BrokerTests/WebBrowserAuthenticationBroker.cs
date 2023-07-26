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

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

using Finebits.Authorization.OAuth2.Brokers;
using Finebits.Authorization.OAuth2.Brokers.Abstractions;
using Finebits.Authorization.OAuth2.Types;

using Moq;
using Moq.Protected;

namespace Finebits.Authorization.OAuth2.Test.BrokerTests;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class WebBrowserAuthenticationBrokerTests
{
    [Test]
    public void Constructor_NullParam_Exception()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new WebBrowserAuthenticationBroker(null, null));
        Assert.That(exception.ParamName, Is.EqualTo("launcher"));

        exception = Assert.Throws<ArgumentNullException>(() => new WebBrowserAuthenticationBroker(null, new Mock<AuthenticationListener>().Object));
        Assert.That(exception.ParamName, Is.EqualTo("launcher"));

        exception = Assert.Throws<ArgumentNullException>(() => new WebBrowserAuthenticationBroker(new Mock<IWebBrowserLauncher>().Object, null));
        Assert.That(exception.ParamName, Is.EqualTo("listener"));
    }

    [Test]
    [TestCase(null, null, "requestUri")]
    [TestCase(null, "https://callback", "requestUri")]
    [TestCase("https://request", null, "callbackUri")]
    public void AuthenticateAsync_NullParam_Exception(string requestStringUri, string callbackStringUri, string paramName)
    {
        var broker = new WebBrowserAuthenticationBroker(new Mock<IWebBrowserLauncher>().Object, new Mock<AuthenticationListener>().Object);

        Uri? requestUri = string.IsNullOrEmpty(requestStringUri) ? null : new(requestStringUri);
        Uri? callbackUri = string.IsNullOrEmpty(callbackStringUri) ? null : new(callbackStringUri);

        var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await broker.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo(paramName));
    }

    [Test]
    public async Task AuthenticateAsync_Launched_Success()
    {
        var mockLauncher = new Mock<IWebBrowserLauncher>();
        mockLauncher.Setup(m => m.LaunchAsync(It.IsAny<Uri>())).Returns(Task.FromResult(true));

        var mockListener = new Mock<AuthenticationListener>();
        mockListener.Protected().Setup<Task<AuthenticationResult>>("GetResultAsync").ReturnsAsync(new AuthenticationResult(new Mock<NameValueCollection>().Object));

        var broker = new WebBrowserAuthenticationBroker(mockLauncher.Object, mockListener.Object);

        var result = await broker.AuthenticateAsync(new Uri("https://request/"), new Uri("https://callback/")).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.EqualTo(AuthenticationResult.Canceled));
            Assert.That(result.Properties, Is.Not.Null);
        });
    }

    [Test]
    public void AuthenticateAsync_NonLaunched_Success()
    {
        var mockLauncher = new Mock<IWebBrowserLauncher>();
        mockLauncher.Setup(m => m.LaunchAsync(It.IsAny<Uri>())).Returns(Task.FromResult(false));

        var mockListener = new Mock<AuthenticationListener>();
        mockListener.Protected().Setup<Task<AuthenticationResult>>("GetResultAsync").ReturnsAsync(new AuthenticationResult(new Mock<NameValueCollection>().Object));

        var broker = new WebBrowserAuthenticationBroker(mockLauncher.Object, mockListener.Object);

        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await broker.AuthenticateAsync(new Uri("https://request/"), new Uri("https://callback/")).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
    }
}
