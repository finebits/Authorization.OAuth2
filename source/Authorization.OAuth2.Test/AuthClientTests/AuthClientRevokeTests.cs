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

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Unit Test Naming Conventions")]
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
[TestFixtureSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.RevocableFixtureData))]
internal class AuthClientRevokeTests
{
    private Test.Data.AuthClientType AuthType { get; init; }

    public AuthClientRevokeTests(Test.Data.AuthClientType authType)
    {
        AuthType = authType;
    }

    [Test]
    public void RevokeAsync_NullParam_Exception()
    {
        Mock<HttpClient> mockHttpClient = new();
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, mockHttpClient.Object, mockAuthBroker.Object, config);

        IRevocable? revocableClient = client as IRevocable;
        Assert.That(revocableClient, Is.Not.Null);

        ArgumentNullException? exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await revocableClient.RevokeAsync(null).ConfigureAwait(false));
        Assert.That(exception.ParamName, Is.EqualTo("credential"));
    }

    [Test]
    public void RevokeAsync_CorrectRequest_Success()
    {
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateSuccess().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Credential credential = Test.Data.AuthCreator.CreateFakeCredential();

        IRevocable? revocableClient = client as IRevocable;
        Assert.That(revocableClient, Is.Not.Null);

        Assert.DoesNotThrowAsync(async () => await revocableClient.RevokeAsync(credential).ConfigureAwait(false));
    }

    [Test]
    public void RevokeAsync_CancellationToken_Exception()
    {
        using CancellationTokenSource cts = new();
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateSuccess().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Credential credential = Test.Data.AuthCreator.CreateFakeCredential();

        IRevocable? revocableClient = client as IRevocable;
        Assert.That(revocableClient, Is.Not.Null);

        cts.Cancel();
        OperationCanceledException? exception = Assert.CatchAsync<OperationCanceledException>(async () => await revocableClient.RevokeAsync(credential, cts.Token).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void RevokeAsync_RequestCancellationToken_Exception()
    {
        using CancellationTokenSource cts = new();
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateCancellationToken(cts).Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Credential credential = Test.Data.AuthCreator.CreateFakeCredential();

        IRevocable? revocableClient = client as IRevocable;
        Assert.That(revocableClient, Is.Not.Null);

        OperationCanceledException? exception = Assert.CatchAsync<OperationCanceledException>(async () => await revocableClient.RevokeAsync(credential, cts.Token).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void RevokeAsync_HttpInvalidResponse_Exception()
    {
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateInvalidResponse().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Credential credential = Test.Data.AuthCreator.CreateFakeCredential();

        IRevocable? revocableClient = client as IRevocable;
        Assert.That(revocableClient, Is.Not.Null);

        AuthorizationInvalidResponseException? exception = Assert.ThrowsAsync<AuthorizationInvalidResponseException>(async () => await revocableClient.RevokeAsync(credential).ConfigureAwait(false));

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
    public void RevokeAsync_HttpBadRequest_Exception()
    {
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateHttpError().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Credential credential = Test.Data.AuthCreator.CreateFakeCredential();

        IRevocable? revocableClient = client as IRevocable;
        Assert.That(revocableClient, Is.Not.Null);

        AuthorizationInvalidResponseException? exception = Assert.ThrowsAsync<AuthorizationInvalidResponseException>(async () => await revocableClient.RevokeAsync(credential).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        HttpRequestException? innerException = exception.InnerException as HttpRequestException;
        Assert.That(innerException, Is.Not.Null);
        Assert.That(innerException.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public void RevokeAsync_HttpEmptyContent_Success()
    {
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateEmptyResponse().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Credential credential = Test.Data.AuthCreator.CreateFakeCredential();

        IRevocable? revocableClient = client as IRevocable;
        Assert.That(revocableClient, Is.Not.Null);

        Assert.DoesNotThrowAsync(async () => await revocableClient.RevokeAsync(credential).ConfigureAwait(false));
    }
}
