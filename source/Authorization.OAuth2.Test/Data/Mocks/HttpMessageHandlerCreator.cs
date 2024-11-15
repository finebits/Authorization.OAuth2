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

using System.Net;
using System.Net.Http.Json;
using System.Text;

using Moq;
using Moq.Protected;

namespace Finebits.Authorization.OAuth2.Test.Data.Mocks
{
    internal static class HttpMessageHandlerCreator
    {
        public static Mock<HttpMessageHandler> CreateSuccess()
        {
            Mock<HttpMessageHandler> mock = new();

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
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("token-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = FakeConstant.Token.AccessToken,
                            token_type = FakeConstant.Token.TokenType,
                            expires_in = FakeConstant.Token.ExpiresIn,
                            refresh_token = FakeConstant.Token.RefreshToken,
                            scope = FakeConstant.Token.Scope,
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("google", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("token-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = FakeConstant.Token.AccessToken,
                            token_type = FakeConstant.Token.TokenType,
                            expires_in = FakeConstant.Token.ExpiresIn,
                            refresh_token = FakeConstant.Token.RefreshToken,
                            scope = FakeConstant.Token.Scope,
                            id_token = FakeConstant.Token.IdToken,
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("refresh-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = FakeConstant.Token.NewAccessToken,
                            token_type = FakeConstant.Token.TokenType,
                            expires_in = FakeConstant.Token.ExpiresIn,
                            refresh_token = FakeConstant.Token.NewRefreshToken,
                            scope = FakeConstant.Token.Scope,
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("google", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("refresh-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = FakeConstant.Token.NewAccessToken,
                            token_type = FakeConstant.Token.TokenType,
                            expires_in = FakeConstant.Token.ExpiresIn,
                            scope = FakeConstant.Token.Scope,
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("revoke-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new { })
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("google", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("profile-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            sub = FakeConstant.UserProfile.Id,
                            name = FakeConstant.UserProfile.Google.Name,
                            given_name = FakeConstant.UserProfile.DisplayName,
                            family_name = FakeConstant.UserProfile.Google.FamilyName,
                            picture = FakeConstant.UserProfile.Google.Picture,
                            email = FakeConstant.UserProfile.Email,
                            email_verified = FakeConstant.UserProfile.Google.EmailVerified,
                            locale = FakeConstant.UserProfile.Google.Locale
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("microsoft", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("profile-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            id = FakeConstant.UserProfile.Id,
                            mail = FakeConstant.UserProfile.Email,
                            displayName = FakeConstant.UserProfile.DisplayName,
                            givenName = FakeConstant.UserProfile.Microsoft.GivenName,
                            surname = FakeConstant.UserProfile.Microsoft.Surname,
                            userPrincipalName = FakeConstant.UserProfile.Microsoft.UserPrincipalName,
                            preferredLanguage = FakeConstant.UserProfile.Microsoft.PreferredLanguage,
                            mobilePhone = FakeConstant.UserProfile.Microsoft.MobilePhone,
                            jobTitle = FakeConstant.UserProfile.Microsoft.JobTitle,
                            officeLocation = FakeConstant.UserProfile.Microsoft.OfficeLocation,
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("outlook", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("profile-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        inputValue: new
                        {
                            Id = FakeConstant.UserProfile.Id,
                            EmailAddress = FakeConstant.UserProfile.Email,
                            DisplayName = FakeConstant.UserProfile.DisplayName,
                            Alias = FakeConstant.UserProfile.Outlook.Alias,
                            MailboxGuid = FakeConstant.UserProfile.Outlook.MailboxGuid,
                        },
                        options: new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.AbsolutePath.EndsWith("avatar-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(FakeConstant.UserProfile.Avatar))),
                });

            return mock;
        }

        public static Mock<HttpMessageHandler> CreateCancellationToken(CancellationTokenSource cts)
        {
            Mock<HttpMessageHandler> mock = new();

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
            Mock<HttpMessageHandler> mock = new();

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = JsonContent.Create(
                        new
                        {
                            error = FakeConstant.Error,
                            error_description = FakeConstant.ErrorDescription,
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("microsoft", StringComparison.Ordinal)
                                               && (rm.RequestUri.AbsolutePath.EndsWith("profile-uri") ||
                                                   rm.RequestUri.AbsolutePath.EndsWith("avatar-uri"))),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = JsonContent.Create(
                        new
                        {
                            error = new
                            {
                                code = FakeConstant.Error,
                                message = FakeConstant.ErrorDescription,
                            }
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("outlook", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("profile-uri")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = JsonContent.Create(
                        new
                        {
                            error = new
                            {
                                code = FakeConstant.Error,
                                message = FakeConstant.ErrorDescription,
                            }
                        }),
                });

            return mock;
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
            Mock<HttpMessageHandler> mock = new();
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
