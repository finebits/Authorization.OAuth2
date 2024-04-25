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

using Finebits.Authorization.OAuth2.Abstractions;

namespace Finebits.Authorization.OAuth2.Exceptions
{
    public sealed class AuthorizationInvalidResponseException : AuthorizationException
    {
        public string ErrorDescription { get; private set; }
        public string ErrorReason { get; private set; }
        public IInvalidResponse ResponseDetails { get; private set; }

        public static readonly string DefaultMessage = "Authorization cannot be done. The service response contains an error.";

        public AuthorizationInvalidResponseException()
        { }

        public AuthorizationInvalidResponseException(string message)
            : base(message)
        { }

        public AuthorizationInvalidResponseException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public AuthorizationInvalidResponseException(IInvalidResponse responseDetails, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorReason = responseDetails?.ErrorReason;
            ErrorDescription = responseDetails?.ErrorDescription;
            ResponseDetails = responseDetails;
        }

        public AuthorizationInvalidResponseException(IInvalidResponse responseDetails, string message)
            : base(message)
        {
            ErrorReason = responseDetails?.ErrorReason;
            ErrorDescription = responseDetails?.ErrorDescription;
            ResponseDetails = responseDetails;
        }

        public AuthorizationInvalidResponseException(IInvalidResponse responseDetails, Exception innerException)
            : base(DefaultMessage, innerException)
        {
            ErrorReason = responseDetails?.ErrorReason;
            ErrorDescription = responseDetails?.ErrorDescription;
            ResponseDetails = responseDetails;
        }

        public AuthorizationInvalidResponseException(IInvalidResponse responseDetails)
            : base(DefaultMessage)
        {
            ErrorReason = responseDetails?.ErrorReason;
            ErrorDescription = responseDetails?.ErrorDescription;
            ResponseDetails = responseDetails;
        }
    }
}
