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
using System.Net;

using Finebits.Authorization.OAuth2.AuthenticationBroker;
using Finebits.Authorization.OAuth2.AuthenticationBroker.Abstractions;
using Finebits.Authorization.OAuth2.Types;

using Moq;

namespace Finebits.Authorization.OAuth2.Test.BrokerTests;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class DesktopAuthenticationBrokerTests
{
    [Test]
    public void Constructor_NullParam_Exception()
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => new DesktopAuthenticationBroker(null));
        Assert.That(exception.ParamName, Is.EqualTo("launcher"));
    }

    [Test]
    [TestCase(null, null, "requestUri")]
    [TestCase(null, "https://callback", "requestUri")]
    [TestCase("https://request", null, "callbackUri")]
    public void AuthenticateAsync_NullParam_Exception(string? requestStringUri, string? callbackStringUri, string paramName)
    {
        DesktopAuthenticationBroker broker = new DesktopAuthenticationBroker(new Mock<IWebBrowserLauncher>().Object);

        Uri? requestUri = string.IsNullOrEmpty(requestStringUri) ? null : new(requestStringUri);
        Uri? callbackUri = string.IsNullOrEmpty(callbackStringUri) ? null : new(callbackStringUri);

        ArgumentNullException? exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await broker.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo(paramName));
    }

    [Test]
    public void AuthenticateAsync_NonLaunched_Exception()
    {
        Mock<IWebBrowserLauncher> mockLauncher = new Mock<IWebBrowserLauncher>();
        mockLauncher.Setup(m => m.LaunchAsync(It.IsAny<Uri>())).Returns(Task.FromResult(false));

        DesktopAuthenticationBroker broker = new DesktopAuthenticationBroker(mockLauncher.Object);
        Uri callback = DesktopAuthenticationBroker.GetLoopbackUri();

        InvalidOperationException? exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await broker.AuthenticateAsync(new Uri("https://request/"), callback).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void AuthenticateAsync_CancellationToken_Exception()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        Mock<IWebBrowserLauncher> mockLauncher = new Mock<IWebBrowserLauncher>();
        mockLauncher.Setup(m => m.LaunchAsync(It.IsAny<Uri>())).Returns(Task.FromResult(true));

        DesktopAuthenticationBroker broker = new DesktopAuthenticationBroker(mockLauncher.Object);
        Uri callback = DesktopAuthenticationBroker.GetLoopbackUri();

        cts.Cancel();
        AuthenticationResult? result = null;
        OperationCanceledException? exception = Assert.ThrowsAsync<OperationCanceledException>(async () => result = await broker.AuthenticateAsync(new Uri("https://request/"), callback, cts.Token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.CancellationToken, Is.EqualTo(cts.Token));
    }

    [Test]
    public void AuthenticateAsync_CancellationTokenDelay_Exception()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        Mock<IWebBrowserLauncher> mockLauncher = new Mock<IWebBrowserLauncher>();
        mockLauncher.Setup(m => m.LaunchAsync(It.IsAny<Uri>())).Returns(Task.FromResult(true));

        DesktopAuthenticationBroker broker = new DesktopAuthenticationBroker(mockLauncher.Object);
        Uri callback = DesktopAuthenticationBroker.GetLoopbackUri();

        cts.CancelAfter(500);
        AuthenticationResult? result = null;
        OperationCanceledException? exception = Assert.ThrowsAsync<OperationCanceledException>(async () => result = await broker.AuthenticateAsync(new Uri("https://request/"), callback, cts.Token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.CancellationToken, Is.EqualTo(cts.Token));
    }

    [Test]
    public void GetRandomUnusedPort_Call_Success()
    {
        int port = int.MinValue;

        Assert.DoesNotThrow(() => port = DesktopAuthenticationBroker.GetRandomUnusedPort());

        Assert.That(port, Is.InRange(IPEndPoint.MinPort, IPEndPoint.MaxPort));
    }

    [Test]
    public void GetDefaultCallbackUri_Call_Success()
    {
        Uri? uri = null;

        Assert.DoesNotThrow(() => uri = DesktopAuthenticationBroker.GetLoopbackUri());

        Assert.That(uri, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(uri.IsAbsoluteUri, Is.True);
            Assert.That(uri.IsLoopback, Is.True);
            Assert.That(uri.Port, Is.InRange(IPEndPoint.MinPort, IPEndPoint.MaxPort));
        });
    }

    [Test]
    public void IsSupported_Call_NotException()
    {
        Assert.DoesNotThrow(() => _ = DesktopAuthenticationBroker.IsSupported);
    }
}
