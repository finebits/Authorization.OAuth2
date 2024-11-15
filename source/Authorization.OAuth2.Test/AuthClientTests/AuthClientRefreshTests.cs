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

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.Test.Data.Mocks;

using Moq;

namespace Finebits.Authorization.OAuth2.Test.AuthClientTests;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
[TestFixtureSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.RefreshableFixtureData))]
internal class AuthClientRefreshTests
{
    private Test.Data.AuthClientType AuthType { get; init; }

    public AuthClientRefreshTests(Test.Data.AuthClientType authType)
    {
        AuthType = authType;
    }

    [Test]
    public void RefreshTokenAsync_NullParam_Exception()
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, mockHttpClient.Object, mockAuthBroker.Object, config);

        IRefreshable? refreshClient = client as IRefreshable;
        Assert.That(refreshClient, Is.Not.Null);

        ArgumentNullException? exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await refreshClient.RefreshTokenAsync(null).ConfigureAwait(false));
        Assert.That(exception.ParamName, Is.EqualTo("token"));
    }

    [Test]
    public async Task RefreshTokenAsync_CorrectRequest_Success()
    {
        using HttpClient httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IRefreshable? refreshClient = client as IRefreshable;
        Assert.That(refreshClient, Is.Not.Null);

        Types.AuthorizationToken newToken = await refreshClient.RefreshTokenAsync(token).ConfigureAwait(false);

        Assert.That(newToken, Is.Not.Null);
        Assert.That(newToken.AccessToken, Is.EqualTo(FakeConstant.Token.NewAccessToken));
    }

    [Test]
    public void RefreshTokenAsync_CancellationToken_Exception()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        using HttpClient httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IRefreshable? refreshClient = client as IRefreshable;
        Assert.That(refreshClient, Is.Not.Null);

        cts.Cancel();
        OperationCanceledException? exception = Assert.CatchAsync<OperationCanceledException>(async () => await refreshClient.RefreshTokenAsync(token, cts.Token).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void RefreshTokenAsync_RequestCancellationToken_Exception()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        using HttpClient httpClient = new HttpClient(HttpMessageHandlerCreator.CreateCancellationToken(cts).Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IRefreshable? refreshClient = client as IRefreshable;
        Assert.That(refreshClient, Is.Not.Null);

        OperationCanceledException? exception = Assert.CatchAsync<OperationCanceledException>(async () => await refreshClient.RefreshTokenAsync(token, cts.Token).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void RefreshTokenAsync_HttpInvalidResponse_Exception()
    {
        using HttpClient httpClient = new HttpClient(HttpMessageHandlerCreator.CreateInvalidResponse().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IRefreshable? refreshClient = client as IRefreshable;
        Assert.That(refreshClient, Is.Not.Null);

        AuthorizationInvalidResponseException? exception = Assert.ThrowsAsync<AuthorizationInvalidResponseException>(async () => await refreshClient.RefreshTokenAsync(token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(exception.ErrorReason, Is.Not.Null);
            Assert.That(exception.ErrorDescription, Is.Not.Null);
        });

        HttpRequestException? innerException = exception.InnerException as HttpRequestException;
        Assert.That(innerException, Is.Not.Null);
        Assert.That(innerException.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public void RefreshTokenAsync_HttpBadRequest_Exception()
    {
        using HttpClient httpClient = new HttpClient(HttpMessageHandlerCreator.CreateHttpError().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IRefreshable? refreshClient = client as IRefreshable;
        Assert.That(refreshClient, Is.Not.Null);

        AuthorizationInvalidResponseException? exception = Assert.ThrowsAsync<AuthorizationInvalidResponseException>(async () => await refreshClient.RefreshTokenAsync(token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        HttpRequestException? innerException = exception.InnerException as HttpRequestException;
        Assert.That(innerException, Is.Not.Null);
        Assert.That(innerException.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public void RefreshTokenAsync_HttpEmptyContent_Exception()
    {
        using HttpClient httpClient = new HttpClient(HttpMessageHandlerCreator.CreateEmptyResponse().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IRefreshable? refreshClient = client as IRefreshable;
        Assert.That(refreshClient, Is.Not.Null);

        AuthorizationEmptyResponseException? exception = Assert.ThrowsAsync<AuthorizationEmptyResponseException>(async () => await refreshClient.RefreshTokenAsync(token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
    }
}
