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

using System.Text.Json.Serialization;

using Finebits.Authorization.OAuth2.Abstractions;

namespace Finebits.Authorization.OAuth2.Microsoft
{
    public partial class MicrosoftAuthClient
    {
        protected class MicrosoftEmptyContent : IMicrosoftInvalidResponse, IInvalidResponse
        {
            [JsonInclude]
            [JsonPropertyName("error")]
            public ResponseError Error { get; private set; }

            public string ErrorDescription => Error?.Message;
            public string ErrorReason => Error?.Code;
            public IResponseError ResponseError => Error;
        }

        protected class MicrosoftProfileContent : MicrosoftEmptyContent, IMicrosoftInvalidResponse, IInvalidResponse
        {
            [JsonInclude]
            [JsonPropertyName("id")]
            public string Id { get; private set; }

            [JsonInclude]
            [JsonPropertyName("mail")]
            public string Mail { get; private set; }

            [JsonInclude]
            [JsonPropertyName("displayName")]
            public string DisplayName { get; private set; }

            [JsonInclude]
            [JsonPropertyName("givenName")]
            public string GivenName { get; private set; }

            [JsonInclude]
            [JsonPropertyName("surname")]
            public string Surname { get; private set; }

            [JsonInclude]
            [JsonPropertyName("userPrincipalName")]
            public string UserPrincipalName { get; private set; }

            [JsonInclude]
            [JsonPropertyName("preferredLanguage")]
            public string PreferredLanguage { get; private set; }

            [JsonInclude]
            [JsonPropertyName("mobilePhone")]
            public string MobilePhone { get; private set; }

            [JsonInclude]
            [JsonPropertyName("jobTitle")]
            public string JobTitle { get; private set; }

            [JsonInclude]
            [JsonPropertyName("officeLocation")]
            public string OfficeLocation { get; private set; }
        }

        protected class ResponseError : IResponseError
        {
            [JsonInclude]
            [JsonPropertyName("code")]
            public string Code { get; private set; }

            [JsonInclude]
            [JsonPropertyName("message")]
            public string Message { get; private set; }

            [JsonInclude]
            [JsonPropertyName("innerError")]
            public InnerError InnerErrorInformation { get; private set; }

            public IInnerError InnerError => InnerErrorInformation;
        }

        protected class InnerError : IInnerError
        {
            [JsonInclude]
            [JsonPropertyName("date")]
            public string RequestDate { get; private set; }

            [JsonInclude]
            [JsonPropertyName("request-id")]
            public string RequestId { get; private set; }

            [JsonInclude]
            [JsonPropertyName("client-request-id")]
            public string ClientRequestId { get; private set; }
        }
    }
}
