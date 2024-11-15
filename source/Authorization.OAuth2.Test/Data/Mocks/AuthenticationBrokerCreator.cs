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

using System.Collections.Specialized;
using System.Web;

using Finebits.Authorization.OAuth2.Abstractions;
using Finebits.Authorization.OAuth2.Types;

using Moq;

namespace Finebits.Authorization.OAuth2.Test.Data.Mocks
{
    internal static class AuthenticationBrokerCreator
    {
        internal static Mock<IAuthenticationBroker> CreateSuccessBroker()
        {
            return CreateBroker(state: null);
        }

        internal static Mock<IAuthenticationBroker> CreateCanceledBroker()
        {
            return CreateBroker(() => throw new OperationCanceledException());
        }

        internal static Mock<IAuthenticationBroker> CreateInvalidDataBroker()
        {
            static AuthenticationResult GetInvalidResult()
            {
                NameValueCollection properties = new()
                {
                    { "error", FakeConstant.Error },
                    { "error_subcode", FakeConstant.ErrorSubcode },
                    { "error_description", FakeConstant.ErrorDescription }
                };

                return new AuthenticationResult(properties);
            }

            return CreateBroker(() => Task.FromResult(GetInvalidResult()));
        }

        internal static Mock<IAuthenticationBroker> CreateEmptyDataBroker()
        {
            return CreateBroker(() => Task.FromResult(new AuthenticationResult([])));
        }

        internal static Mock<IAuthenticationBroker> CreateMissingDataBroker()
        {
            static AuthenticationResult GetMissingResult(Uri requestUri)
            {
                NameValueCollection query = HttpUtility.ParseQueryString(requestUri.Query);

                NameValueCollection properties = new()
                {
                    { "state", query["state"] }
                };

                return new AuthenticationResult(properties);
            }

            Uri request = new("https://request");

            Mock<IAuthenticationBroker> mock = new();
            mock.Setup(broker => broker.AuthenticateAsync(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback<Uri, Uri, CancellationToken>((r, _, _) => request = r)
                .Returns(() => Task.FromResult(GetMissingResult(request)));

            return mock;
        }

        internal static Mock<IAuthenticationBroker> CreateWrongDataBroker()
        {
            return CreateBroker(state: "wrong-state");
        }

        internal static Mock<IAuthenticationBroker> CreateThrowExceptionBroker()
        {
            return CreateBroker(() => throw new InvalidOperationException());
        }

        private static Mock<IAuthenticationBroker> CreateBroker(Func<Task<AuthenticationResult>> valueFunction)
        {
            Mock<IAuthenticationBroker> mock = new();
            mock.Setup(broker => broker.AuthenticateAsync(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Returns(valueFunction);

            return mock;
        }

        private static Mock<IAuthenticationBroker> CreateBroker(string? state)
        {
            static AuthenticationResult GetSuccessResult(Uri requestUri, string? state)
            {
                NameValueCollection query = HttpUtility.ParseQueryString(requestUri.Query);

                NameValueCollection properties = new()
                {
                    { "code", "fake-code" },
                    { "state", state ?? query["state"] }
                };

                return new AuthenticationResult(properties);
            }

            Uri request = new("https://request");

            Mock<IAuthenticationBroker> mock = new();
            mock.Setup(broker => broker.AuthenticateAsync(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback<Uri, Uri, CancellationToken>((r, _, _) => request = r)
                .Returns(() => Task.FromResult(GetSuccessResult(request, state)));

            return mock;
        }
    }
}
