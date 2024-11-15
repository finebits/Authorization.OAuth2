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
using Finebits.Authorization.OAuth2.Google;
using Finebits.Authorization.OAuth2.Microsoft;
using Finebits.Authorization.OAuth2.Outlook;
using Finebits.Authorization.OAuth2.Test.Data.Mocks;

using Moq;

namespace Finebits.Authorization.OAuth2.Test.AuthClientTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Unit Test Naming Conventions")]
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
[TestFixtureSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.ProfileReaderFixtureData))]
internal class AuthClientReadProfileTests
{
    private Test.Data.AuthClientType AuthType { get; init; }

    public AuthClientReadProfileTests(Test.Data.AuthClientType authType)
    {
        AuthType = authType;
    }

    [Test]
    public void ReadProfileAsync_NullParam_Exception()
    {
        Mock<HttpClient> mockHttpClient = new();
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, mockHttpClient.Object, mockAuthBroker.Object, config);

        IProfileReader? profileReader = client as IProfileReader;
        Assert.That(profileReader, Is.Not.Null);

        ArgumentNullException? exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await profileReader.ReadProfileAsync(null).ConfigureAwait(false));
        Assert.That(exception.ParamName, Is.EqualTo("token"));
    }

    [Test]
    public void ReadProfileAsync_CorrectRequest_Success()
    {
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateSuccess().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IProfileReader? profileReader = client as IProfileReader;
        Assert.That(profileReader, Is.Not.Null);

        IUserProfile? userProfile = null;

        Assert.DoesNotThrowAsync(async () => userProfile = await profileReader.ReadProfileAsync(token).ConfigureAwait(false));

        Assert.That(userProfile, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(userProfile is MicrosoftUserProfile, client is MicrosoftAuthClient ? Is.True : Is.False);
            Assert.That(userProfile is GoogleUserProfile, client is GoogleAuthClient ? Is.True : Is.False);
            Assert.That(userProfile is IUserAvatar, client is GoogleAuthClient ? Is.True : Is.False);
            Assert.That(userProfile.Id, Is.EqualTo(FakeConstant.UserProfile.Id));
            Assert.That(userProfile.Email, Is.EqualTo(FakeConstant.UserProfile.Email));
            Assert.That(userProfile.DisplayName, Is.EqualTo(FakeConstant.UserProfile.DisplayName));
        });
    }

    [Test]
    public void ReadProfileAsync_CancellationToken_Exception()
    {
        using CancellationTokenSource cts = new();
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateSuccess().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IProfileReader? profileReader = client as IProfileReader;
        Assert.That(profileReader, Is.Not.Null);

        cts.Cancel();
        OperationCanceledException? exception = Assert.CatchAsync<OperationCanceledException>(async () => await profileReader.ReadProfileAsync(token, cts.Token).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void ReadProfileAsync_RequestCancellationToken_Exception()
    {
        using CancellationTokenSource cts = new();
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateCancellationToken(cts).Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IProfileReader? profileReader = client as IProfileReader;
        Assert.That(profileReader, Is.Not.Null);

        OperationCanceledException? exception = Assert.CatchAsync<OperationCanceledException>(async () => await profileReader.ReadProfileAsync(token, cts.Token).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void ReadProfileAsync_HttpInvalidResponse_Exception()
    {
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateInvalidResponse().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IProfileReader? profileReader = client as IProfileReader;
        Assert.That(profileReader, Is.Not.Null);

        AuthorizationInvalidResponseException? exception = Assert.ThrowsAsync<AuthorizationInvalidResponseException>(async () => await profileReader.ReadProfileAsync(token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(exception.ErrorReason, Is.EqualTo(FakeConstant.Error));
            Assert.That(exception.ErrorDescription, Is.EqualTo(FakeConstant.ErrorDescription));
            Assert.That(exception.ResponseDetails, Is.Not.Null);
            Assert.That(exception.ResponseDetails is IMicrosoftInvalidResponse, client is MicrosoftAuthClient ? Is.True : Is.False);
            Assert.That(exception.ResponseDetails is IOutlookInvalidResponse, client is OutlookAuthClient ? Is.True : Is.False);
        });

        HttpRequestException? innerException = exception.InnerException as HttpRequestException;
        Assert.That(innerException, Is.Not.Null);
        Assert.That(innerException.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public void ReadProfileAsync_HttpBadRequest_Exception()
    {
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateHttpError().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IProfileReader? profileReader = client as IProfileReader;
        Assert.That(profileReader, Is.Not.Null);

        AuthorizationInvalidResponseException? exception = Assert.ThrowsAsync<AuthorizationInvalidResponseException>(async () => await profileReader.ReadProfileAsync(token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
        HttpRequestException? innerException = exception.InnerException as HttpRequestException;
        Assert.That(innerException, Is.Not.Null);
        Assert.That(innerException.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public void ReadProfileAsync_HttpEmptyContent_Exception()
    {
        using HttpClient httpClient = new(HttpMessageHandlerCreator.CreateEmptyResponse().Object);
        Mock<IAuthenticationBroker> mockAuthBroker = new();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(AuthType);
        IAuthorizationClient client = Test.Data.AuthCreator.CreateAuthClient(AuthType, httpClient, mockAuthBroker.Object, config);
        Types.Token token = Test.Data.AuthCreator.CreateFakeToken();

        IProfileReader? profileReader = client as IProfileReader;
        Assert.That(profileReader, Is.Not.Null);

        AuthorizationEmptyResponseException? exception = Assert.ThrowsAsync<AuthorizationEmptyResponseException>(async () => await profileReader.ReadProfileAsync(token).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
    }
}
