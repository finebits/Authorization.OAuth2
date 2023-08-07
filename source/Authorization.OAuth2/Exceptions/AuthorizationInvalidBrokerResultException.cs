﻿// ---------------------------------------------------------------------------- //
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

using System;
using System.Collections.Specialized;

namespace Finebits.Authorization.OAuth2.Exceptions
{
    public sealed class AuthorizationInvalidBrokerResultException : AuthorizationException
    {
        public string Error { get; private set; }
        public string ErrorDescription { get; private set; }
        public NameValueCollection Properties { get; private set; }

        public AuthorizationInvalidBrokerResultException()
            : base(ErrorType.InvalidResponse)
        { }

        public AuthorizationInvalidBrokerResultException(string message)
            : base(ErrorType.InvalidResponse, message)
        { }

        public AuthorizationInvalidBrokerResultException(string message, Exception innerException)
            : base(ErrorType.InvalidResponse, message, innerException)
        { }

        public AuthorizationInvalidBrokerResultException(NameValueCollection properties, string message, Exception innerException)
            : base(ErrorType.InvalidResponse, message, innerException)
        {
            Properties = properties;
            Error = Properties?["error"];
            ErrorDescription = Properties?["error_description"];
        }
    }
}
