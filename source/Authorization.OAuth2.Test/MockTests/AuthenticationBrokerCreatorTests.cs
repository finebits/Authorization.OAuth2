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

using Finebits.Authorization.OAuth2.Test.Data.Mocks;
using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Test.MockTests;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class AuthenticationBrokerCreatorTests
{
    [Test]
    public async Task CreateSuccessBroker_BrokerWorkflow_Success()
    {
        var state = "fake-state";
        var requestUri = new Uri($"https://service/auth-uri?state={state}");
        var callbackUri = new Uri("https://redirect");

        var mockBroker = AuthenticationBrokerCreator.CreateSuccessBroker();
        var result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.EqualTo(AuthenticationResult.Canceled));
            Assert.That(result.Properties, Is.Not.Null);
            Assert.That(result.Properties["state"], Is.EqualTo(state));
            Assert.That(result.Properties["code"], Is.Not.Null);
            Assert.That(result.Properties["error"], Is.Null);
            Assert.That(result.Properties["error_description"], Is.Null);
        });
    }

    [Test]
    public async Task CreateCanceledBroker_BrokerWorkflow_Canceled()
    {
        var requestUri = new Uri($"https://service/auth-uri");
        var callbackUri = new Uri("https://redirect");

        var mockBroker = AuthenticationBrokerCreator.CreateCanceledBroker();
        var result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(AuthenticationResult.Canceled));
            Assert.That(result.Properties, Is.Null);
        });
    }

    [Test]
    public async Task CreateInvalidDataBroker_BrokerWorkflow_Error()
    {
        var requestUri = new Uri($"https://service/auth-uri");
        var callbackUri = new Uri("https://redirect");

        var mockBroker = AuthenticationBrokerCreator.CreateInvalidDataBroker();
        var result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.EqualTo(AuthenticationResult.Canceled));
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
        var requestUri = new Uri($"https://service/auth-uri");
        var callbackUri = new Uri("https://redirect");

        var mockBroker = AuthenticationBrokerCreator.CreateThrowExceptionBroker();

        var exception = Assert.CatchAsync<Exception>(async () => await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false));
        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public async Task CreateEmptyDataBroker_BrokerWorkflow_Empty()
    {
        var requestUri = new Uri($"https://service/auth-uri");
        var callbackUri = new Uri("https://redirect");

        var mockBroker = AuthenticationBrokerCreator.CreateEmptyDataBroker();
        var result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.EqualTo(AuthenticationResult.Canceled));
            Assert.That(result.Properties, Is.Not.Null);
            Assert.That(result.Properties, Is.Empty);
        });
    }

    [Test]
    public async Task CreateMissingDataBroker_BrokerWorkflow_CodeMissing()
    {
        var requestUri = new Uri($"https://service/auth-uri");
        var callbackUri = new Uri("https://redirect");

        var mockBroker = AuthenticationBrokerCreator.CreateMissingDataBroker();
        var result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.EqualTo(AuthenticationResult.Canceled));
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
        var state = "fake-state";
        var requestUri = new Uri($"https://service/auth-uri?state={state}");
        var callbackUri = new Uri("https://redirect");

        var mockBroker = AuthenticationBrokerCreator.CreateWrongDataBroker();
        var result = await mockBroker.Object.AuthenticateAsync(requestUri, callbackUri).ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.EqualTo(AuthenticationResult.Canceled));
            Assert.That(result.Properties, Is.Not.Null);
            Assert.That(result.Properties, Is.Not.Empty);
            Assert.That(result.Properties["state"], Is.Not.EqualTo(state));
            Assert.That(result.Properties["code"], Is.Not.Null);
            Assert.That(result.Properties["error"], Is.Null);
            Assert.That(result.Properties["error_description"], Is.Null);
        });
    }
}
