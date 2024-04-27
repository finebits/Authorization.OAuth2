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

using System;
using System.Text.Json.Serialization;

using Finebits.Authorization.OAuth2.Abstractions;

namespace Finebits.Authorization.OAuth2.Outlook
{
    public partial class OutlookAuthClient
    {
        protected class OutlookEmptyContent : IOutlookInvalidResponse, IInvalidResponse
        {
            [JsonInclude]
            [JsonPropertyName("error")]
            public ResponseError Error { get; private set; }

            public string ErrorDescription => Error?.Message;
            public string ErrorReason => Error?.Code;
            public IOutlookResponseError ResponseError => Error;
        }

        protected class OutlookProfileContent : OutlookEmptyContent, IOutlookInvalidResponse, IInvalidResponse
        {
            [JsonInclude]
            [JsonPropertyName("Id")]
            public string Id { get; private set; }

            [JsonInclude]
            [JsonPropertyName("EmailAddress")]
            public string Mail { get; private set; }

            [JsonInclude]
            [JsonPropertyName("DisplayName")]
            public string DisplayName { get; private set; }

            [JsonInclude]
            [JsonPropertyName("Alias")]
            public string Alias { get; private set; }

            [JsonInclude]
            [JsonPropertyName("MailboxGuid")]
            public string MailboxGuid { get; private set; }
        }

        protected class ResponseError : IOutlookResponseError
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

            public IOutlookInnerError InnerError => InnerErrorInformation;
        }

        protected class InnerError : IOutlookInnerError
        {
            [JsonInclude]
            [JsonPropertyName("date")]
            public string RequestDate { get; private set; }

            [JsonInclude]
            [JsonPropertyName("requestId")]
            public string RequestId { get; private set; }

            [JsonInclude]
            [JsonPropertyName("errorUrl")]
            public string ErrorLink { get; private set; }

            public Uri ErrorUrl => Uri.TryCreate(ErrorLink, UriKind.RelativeOrAbsolute, out Uri uri) ? uri : null;
        }
    }
}
