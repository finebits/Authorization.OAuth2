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
using System.Net.Mime;

using Finebits.Authorization.OAuth2.Test.Data.Mocks;

namespace Finebits.Authorization.OAuth2.Test.MockTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Unit Test Naming Conventions")]
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class HttpMessageHandlerCreatorTests
{
    [Test]
    [TestCase("http://any")]
    [TestCase("http://google")]
    [TestCase("http://microsoft")]
    [TestCase("http://outlook")]
    public async Task CreateSuccess_TokenRequest_OK(Uri uri)
    {
        using HttpRequestMessage request = new()
        {
            RequestUri = new Uri(uri, "token-uri"),
            Method = HttpMethod.Get,
        };

        Moq.Mock<HttpMessageHandler> creator = HttpMessageHandlerCreator.CreateSuccess();
        using HttpClient httpClient = new(creator.Object);
        using HttpResponseMessage responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);

        Assert.That(responseMessage, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseMessage.IsSuccessStatusCode, Is.True);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });

        Assert.That(responseMessage.Content.Headers.ContentType, Is.Not.Null);
        Assert.That(responseMessage.Content.Headers.ContentType.MediaType, Is.EqualTo(MediaTypeNames.Application.Json));
    }

    [Test]
    [TestCase("http://any")]
    [TestCase("http://google")]
    [TestCase("http://microsoft")]
    [TestCase("http://outlook")]
    public async Task CreateSuccess_RefreshRequest_OK(Uri uri)
    {
        using HttpRequestMessage request = new()
        {
            RequestUri = new Uri(uri, "refresh-uri"),
            Method = HttpMethod.Get,
        };

        Moq.Mock<HttpMessageHandler> creator = HttpMessageHandlerCreator.CreateSuccess();
        using HttpClient httpClient = new(creator.Object);
        using HttpResponseMessage responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);

        Assert.That(responseMessage, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseMessage.IsSuccessStatusCode, Is.True);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });

        Assert.That(responseMessage.Content.Headers.ContentType, Is.Not.Null);
        Assert.That(responseMessage.Content.Headers.ContentType.MediaType, Is.EqualTo(MediaTypeNames.Application.Json));
    }

    [Test]
    public async Task CreateSuccess_SendRequest_Fail()
    {
        using HttpRequestMessage request = new()
        {
            RequestUri = new Uri("http://any/any"),
            Method = HttpMethod.Get,
        };

        Moq.Mock<HttpMessageHandler> creator = HttpMessageHandlerCreator.CreateSuccess();
        using HttpClient httpClient = new(creator.Object);
        using HttpResponseMessage responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);

        Assert.That(responseMessage, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseMessage.IsSuccessStatusCode, Is.False);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });

        Assert.That(responseMessage.Content.Headers.ContentType, Is.Null);
    }

    [Test]
    public async Task CreateInvalidResponse_SendRequest_BadRequest()
    {
        using HttpRequestMessage request = new()
        {
            RequestUri = new Uri("http://any-uri"),
            Method = HttpMethod.Get,
        };

        Moq.Mock<HttpMessageHandler> creator = HttpMessageHandlerCreator.CreateInvalidResponse();
        using HttpClient httpClient = new(creator.Object);
        using HttpResponseMessage responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);

        Assert.That(responseMessage, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseMessage.IsSuccessStatusCode, Is.False);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        });

        Assert.That(responseMessage.Content.Headers.ContentType, Is.Not.Null);
        Assert.That(responseMessage.Content.Headers.ContentType.MediaType, Is.EqualTo(MediaTypeNames.Application.Json));
    }

    [Test]
    public async Task CreateEmptyResponse_SendRequest_EmptyContent()
    {
        using HttpRequestMessage request = new()
        {
            RequestUri = new Uri("http://any-uri"),
            Method = HttpMethod.Get,
        };

        Moq.Mock<HttpMessageHandler> creator = HttpMessageHandlerCreator.CreateEmptyResponse();
        using HttpClient httpClient = new(creator.Object);
        using HttpResponseMessage responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);

        Assert.That(responseMessage, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseMessage.IsSuccessStatusCode, Is.True);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });

        Assert.That(responseMessage.Content.Headers.ContentType, Is.Null);
    }

    [Test]
    public async Task CreateHttpError_SendRequest_BadRequestEmptyContent()
    {
        using HttpRequestMessage request = new()
        {
            RequestUri = new Uri("http://any-uri"),
            Method = HttpMethod.Get,
        };

        Moq.Mock<HttpMessageHandler> creator = HttpMessageHandlerCreator.CreateHttpError();
        using HttpClient httpClient = new(creator.Object);
        using HttpResponseMessage responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);

        Assert.That(responseMessage, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseMessage.IsSuccessStatusCode, Is.False);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        });

        Assert.That(responseMessage.Content.Headers.ContentType, Is.Null);
    }
}
