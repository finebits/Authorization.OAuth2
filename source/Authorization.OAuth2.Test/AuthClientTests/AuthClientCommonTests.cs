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

using Finebits.Authorization.OAuth2.Abstractions;

using Moq;

namespace Finebits.Authorization.OAuth2.Test.AuthClientTests;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class AuthClientCommonTests
{
    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.AuthClientCaseData))]
    public void CreateAuthorizationClient_CorrectParam_Success(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.AuthClientCaseData))]
    public void CreateAuthorizationClient_NullParam_Exception(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);

        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => Test.Data.AuthCreator.CreateAuthClient(clientType, null, null, null));
        Assert.That(exception.ParamName, Is.EqualTo("httpClient"));

        exception = Assert.Throws<ArgumentNullException>(() => Test.Data.AuthCreator.CreateAuthClient(clientType, null, mockAuthBroker.Object, null));
        Assert.That(exception.ParamName, Is.EqualTo("httpClient"));

        exception = Assert.Throws<ArgumentNullException>(() => Test.Data.AuthCreator.CreateAuthClient(clientType, null, null, config));
        Assert.That(exception.ParamName, Is.EqualTo("httpClient"));

        exception = Assert.Throws<ArgumentNullException>(() => Test.Data.AuthCreator.CreateAuthClient(clientType, null, mockAuthBroker.Object, config));
        Assert.That(exception.ParamName, Is.EqualTo("httpClient"));

        exception = Assert.Throws<ArgumentNullException>(() => Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, null, null));
        Assert.That(exception.ParamName, Is.EqualTo("broker"));

        exception = Assert.Throws<ArgumentNullException>(() => Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, null, config));
        Assert.That(exception.ParamName, Is.EqualTo("broker"));

        exception = Assert.Throws<ArgumentNullException>(() => Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, null));
        Assert.That(exception.ParamName, Is.EqualTo("config"));
    }

    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.RefreshableCaseData))]
    public void ConvertIRefreshable_RefreshableClient_Success(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
        Assert.That(client is IRefreshable, Is.True);
    }

    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.NonRefreshableCaseData))]
    public void ConvertIRefreshable_NonRefreshableClient_Fail(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
        Assert.That(client is IRefreshable, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.RevocableCaseData))]
    public void ConvertIRevocable_RevocableClient_Success(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
        Assert.That(client is IRevocable, Is.True);
    }

    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.IrrevocableCaseData))]
    public void ConvertIRevocable_IrrevocableClient_Fail(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
        Assert.That(client is IRevocable, Is.False);
    }


    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.ProfileReaderCaseData))]
    public void ConvertIProfileReader_ProfileReaderClient_Success(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
        Assert.That(client is IProfileReader, Is.True);
    }

    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.NonProfileReaderCaseData))]
    public void ConvertIProfileReader_NonProfileReaderClient_Fail(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
        Assert.That(client is IProfileReader, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.UserAvatarLoaderCaseData))]
    public void ConvertIUserAvatarLoader_UserAvatarLoaderClient_Success(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
        Assert.That(client is IUserAvatarLoader, Is.True);
    }

    [Test]
    [TestCaseSource(typeof(Test.Data.AuthClientDataFixture), nameof(Test.Data.AuthClientDataFixture.NonUserAvatarLoaderCaseData))]
    public void ConvertIUserAvatarLoader_NonUserAvatarLoaderClient_Fail(Test.Data.AuthClientType clientType)
    {
        Mock<HttpClient> mockHttpClient = new Mock<HttpClient>();
        Mock<IAuthenticationBroker> mockAuthBroker = new Mock<IAuthenticationBroker>();
        AuthConfiguration config = Test.Data.AuthCreator.CreateConfig(clientType);
        IAuthorizationClient? client = null;

        Assert.DoesNotThrow(() => client = Test.Data.AuthCreator.CreateAuthClient(clientType, mockHttpClient.Object, mockAuthBroker.Object, config));
        Assert.That(client, Is.Not.Null);
        Assert.That(client is IUserAvatarLoader, Is.False);
    }
}
