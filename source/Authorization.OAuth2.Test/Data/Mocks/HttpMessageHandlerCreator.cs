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

using System.Net;
using System.Net.Http.Json;

using Moq;
using Moq.Protected;

namespace Finebits.Authorization.OAuth2.Test.Data.Mocks
{
    internal static class HttpMessageHandlerCreator
    {
        public static Mock<HttpMessageHandler> CreateSuccess()
        {
            var mock = new Mock<HttpMessageHandler>();

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("token-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = "fake-access-token",
                            token_type = "Bearer",
                            expires_in = 3600,
                            refresh_token = "fake-refresh-token",
                            scope = "email-fake-scope profile-fake-scope",
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("google", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("token-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = "fake-access-token",
                            token_type = "Bearer",
                            expires_in = 3600,
                            refresh_token = "fake-refresh-token",
                            scope = "email-fake-scope profile-fake-scope",
                            id_token = "fake-id-token",
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("refresh-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = "fake-new-access-token",
                            token_type = "Bearer",
                            expires_in = 3600,
                            refresh_token = "fake-new-refresh-token",
                            scope = "email-fake-scope profile-fake-scope",

                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("google", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("refresh-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = "fake-access-token",
                            token_type = "Bearer",
                            expires_in = 3600,
                            refresh_token = "",
                            scope = "email-fake-scope profile-fake-scope",
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("revoke-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new { })
                });

            return mock;
        }

        public static Mock<HttpMessageHandler> CreateCancellationToken(CancellationTokenSource cts)
        {
            var mock = new Mock<HttpMessageHandler>();

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback(() => { cts.Cancel(); })
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new { })
                });

            return mock;
        }

        public static Mock<HttpMessageHandler> CreateHttpError()
        {
            return Create(() => new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest
            });
        }

        public static Mock<HttpMessageHandler> CreateInvalidResponse()
        {
            return Create(() => new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = JsonContent.Create(
                    new
                    {
                        error = "fake-error",
                        error_description = "fake-error-description",
                    }),
            });
        }

        public static Mock<HttpMessageHandler> CreateEmptyResponse()
        {
            return Create(() => new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            });
        }

        private static Mock<HttpMessageHandler> Create(Func<HttpResponseMessage> valueFunction)
        {
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(valueFunction);

            return mock;
        }
    }
}
