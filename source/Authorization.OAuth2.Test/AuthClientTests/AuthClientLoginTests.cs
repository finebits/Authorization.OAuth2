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

using Finebits.Authorization.OAuth2.Exceptions;
using Finebits.Authorization.OAuth2.Google;
using Finebits.Authorization.OAuth2.Test.Data.Mocks;

namespace Finebits.Authorization.OAuth2.Test.AuthClientTests;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
[TestFixtureSource(typeof(Data.AuthClientDataFixture), nameof(Data.AuthClientDataFixture.AuthClientFixtureData))]
internal class AuthClientLoginTests
{
    private Test.Data.AuthClientType AuthType { get; init; }

    public AuthClientLoginTests(Test.Data.AuthClientType authType)
    {
        AuthType = authType;
    }

    [Test]
    public async Task LoginAsync_CorrectRequest_Success()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var authToken = await client.LoginAsync().ConfigureAwait(false);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.AccessToken, Is.EqualTo(FakeConstant.Token.AccessToken));
            Assert.That(authToken.RefreshToken, Is.EqualTo(FakeConstant.Token.RefreshToken));
            Assert.That(authToken.TokenType, Is.EqualTo(FakeConstant.Token.TokenType));
            Assert.That(authToken.ExpiresIn, Is.EqualTo(TimeSpan.FromSeconds(FakeConstant.Token.ExpiresIn)));

            if (authToken is GoogleAuthorizationToken googleToken)
            {
                Assert.That(googleToken.IdToken, Is.Not.Null);
            }
        });
    }

    [Test]
    public async Task LoginAsync_UserName_Success()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var authToken = await client.LoginAsync("UserId").ConfigureAwait(false);

        Assert.That(authToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authToken.AccessToken, Is.EqualTo(FakeConstant.Token.AccessToken));
            Assert.That(authToken.RefreshToken, Is.EqualTo(FakeConstant.Token.RefreshToken));
            Assert.That(authToken.TokenType, Is.EqualTo(FakeConstant.Token.TokenType));
            Assert.That(authToken.ExpiresIn, Is.EqualTo(TimeSpan.FromSeconds(FakeConstant.Token.ExpiresIn)));
        });
    }

    [Test]
    public void LoginAsync_EmptyName_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await client.LoginAsync(string.Empty).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("userId"));
    }

    [Test]
    public void LoginAsync_NullName_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await client.LoginAsync(null).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("userId"));
    }

    [Test]
    public void LoginAsync_CancellationToken_Exception()
    {
        using var cts = new CancellationTokenSource();
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        cts.Cancel();
        var exception = Assert.CatchAsync<OperationCanceledException>(async () => await client.LoginAsync(cts.Token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void LoginAsync_RequestCancellationToken_Exception()
    {
        using var cts = new CancellationTokenSource();
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateCancellationToken(cts).Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.CatchAsync<OperationCanceledException>(async () => await client.LoginAsync(cts.Token).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void LoginAsync_CancelAuthentication_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateCanceledBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<OperationCanceledException>(async () => await client.LoginAsync().ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void LoginAsync_AuthenticationError_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateInvalidDataBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<AuthorizationBrokerResultException>(async () => await client.LoginAsync().ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(exception.Error, Is.EqualTo(FakeConstant.Error));
            Assert.That(exception.ErrorDescription, Is.EqualTo(FakeConstant.ErrorDescription));
            Assert.That(exception.Properties, Is.Not.Null);
            Assert.That(exception.Properties["error_subcode"], Is.EqualTo(FakeConstant.ErrorSubcode));
            Assert.That(exception.InnerException, Is.Null);
        });
    }

    [Test]
    public void LoginAsync_AuthenticationEmpty_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateEmptyDataBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<AuthorizationPropertiesException>(async () => await client.LoginAsync().ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.PropertyName, Is.Null);
    }

    [Test]
    public void LoginAsync_AuthenticationWrongProperty_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateWrongDataBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<AuthorizationPropertiesException>(async () => await client.LoginAsync().ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.PropertyName, Is.EqualTo("state"));
    }

    [Test]
    public void LoginAsync_AuthenticationMissingProperty_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateMissingDataBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<AuthorizationPropertiesException>(async () => await client.LoginAsync().ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.PropertyName, Is.EqualTo("code"));
    }

    [Test]
    public void LoginAsync_AuthenticationInnerException_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateSuccess().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateThrowExceptionBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<AuthorizationException>(async () => await client.LoginAsync().ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.InnerException, Is.Not.Null);
    }

    [Test]
    public void LoginAsync_HttpInvalidResponse_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateInvalidResponse().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<AuthorizationInvalidResponseException>(async () => await client.LoginAsync().ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(exception.ErrorReason, Is.Not.Null);
            Assert.That(exception.ErrorDescription, Is.Not.Null);
        });

        var innerException = exception.InnerException as HttpRequestException;
        Assert.That(innerException, Is.Not.Null);
        Assert.That(innerException.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public void LoginAsync_HttpBadRequest_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateHttpError().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<AuthorizationInvalidResponseException>(async () => await client.LoginAsync().ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);

        var innerException = exception.InnerException as HttpRequestException;
        Assert.That(innerException, Is.Not.Null);
        Assert.That(innerException.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public void LoginAsync_HttpEmptyContent_Exception()
    {
        using var httpClient = new HttpClient(HttpMessageHandlerCreator.CreateEmptyResponse().Object);
        var mockAuthBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var config = Test.Data.AuthCreator.CreateConfig(AuthType);
        var client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);

        var exception = Assert.ThrowsAsync<AuthorizationEmptyResponseException>(async () => await client.LoginAsync().ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
    }
}
