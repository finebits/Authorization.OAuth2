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

using Finebits.Authorization.OAuth2.Test.Data.Mocks;

namespace Finebits.Authorization.OAuth2.Test.MockTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Unit Test Naming Conventions")]
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class AuthenticationBrokerCreatorTests
{
    [Test]
    public async Task CreateSuccessBroker_BrokerWorkflow_Success()
    {
        string state = "fake-state";
        Uri requestUri = new($"https://service/auth-uri?state={state}");
        Uri callbackUri = new("https://redirect");

        Moq.Mock<Abstractions.IAuthenticationBroker> mockBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        Types.AuthenticationResult result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Properties, Is.Not.Null);
            Assert.That(result.Properties["state"], Is.EqualTo(state));
            Assert.That(result.Properties["code"], Is.Not.Null);
            Assert.That(result.Properties["error"], Is.Null);
            Assert.That(result.Properties["error_description"], Is.Null);
        });
    }

    [Test]
    public void CreateCanceledBroker_BrokerWorkflow_Exception()
    {
        Uri requestUri = new($"https://service/auth-uri");
        Uri callbackUri = new("https://redirect");

        Moq.Mock<Abstractions.IAuthenticationBroker> mockBroker = AuthenticationBrokerCreator.CreateCanceledBroker();
        OperationCanceledException? exception = Assert.ThrowsAsync<OperationCanceledException>(async () => await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false));

        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public async Task CreateInvalidDataBroker_BrokerWorkflow_Error()
    {
        Uri requestUri = new($"https://service/auth-uri");
        Uri callbackUri = new("https://redirect");

        Moq.Mock<Abstractions.IAuthenticationBroker> mockBroker = AuthenticationBrokerCreator.CreateInvalidDataBroker();
        Types.AuthenticationResult result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Properties, Is.Not.Null);
            Assert.That(result.Properties["state"], Is.Null);
            Assert.That(result.Properties["code"], Is.Null);
            Assert.That(result.Properties["error"], Is.Not.Null);
            Assert.That(result.Properties["error_description"], Is.Not.Null);
        });
    }

    [Test]
    public void CreateThrowExceptionBroker_BrokerWorkflow_Exception()
    {
        Uri requestUri = new($"https://service/auth-uri");
        Uri callbackUri = new("https://redirect");

        Moq.Mock<Abstractions.IAuthenticationBroker> mockBroker = AuthenticationBrokerCreator.CreateThrowExceptionBroker();

        Exception? exception = Assert.CatchAsync<Exception>(async () => await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public async Task CreateEmptyDataBroker_BrokerWorkflow_Empty()
    {
        Uri requestUri = new($"https://service/auth-uri");
        Uri callbackUri = new("https://redirect");

        Moq.Mock<Abstractions.IAuthenticationBroker> mockBroker = AuthenticationBrokerCreator.CreateEmptyDataBroker();
        Types.AuthenticationResult result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Properties, Is.Not.Null);
            Assert.That(result.Properties, Is.Empty);
        });
    }

    [Test]
    public async Task CreateMissingDataBroker_BrokerWorkflow_CodeMissing()
    {
        Uri requestUri = new($"https://service/auth-uri");
        Uri callbackUri = new("https://redirect");

        Moq.Mock<Abstractions.IAuthenticationBroker> mockBroker = AuthenticationBrokerCreator.CreateMissingDataBroker();
        Types.AuthenticationResult result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Properties, Is.Not.Null);
            Assert.That(result.Properties, Is.Not.Empty);
            Assert.That(result.Properties["code"], Is.Null);
            Assert.That(result.Properties["error"], Is.Null);
            Assert.That(result.Properties["error_description"], Is.Null);
        });
    }

    [Test]
    public async Task CreateWrongDataBroker_BrokerWorkflow_WrongState()
    {
        string state = "fake-state";
        Uri requestUri = new($"https://service/auth-uri?state={state}");
        Uri callbackUri = new("https://redirect");

        Moq.Mock<Abstractions.IAuthenticationBroker> mockBroker = AuthenticationBrokerCreator.CreateWrongDataBroker();
        Types.AuthenticationResult result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Properties, Is.Not.Null);
            Assert.That(result.Properties, Is.Not.Empty);
            Assert.That(result.Properties["state"], Is.Not.EqualTo(state));
            Assert.That(result.Properties["code"], Is.Not.Null);
            Assert.That(result.Properties["error"], Is.Null);
            Assert.That(result.Properties["error_description"], Is.Null);
        });
    }
}
