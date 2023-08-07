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

using System;

namespace Finebits.Authorization.OAuth2.Exceptions
{
    public enum ErrorType
    {
        Error,
        Cancel,
        InvalidResponse,
    }

    public class AuthorizationException : Exception
    {
        public ErrorType ErrorType { get; private set; }

        public AuthorizationException()
        {
            ErrorType = ErrorType.Error;
        }

        public AuthorizationException(string message) : base(message)
        {
            ErrorType = ErrorType.Error;
        }

        public AuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
            ErrorType = ErrorType.Error;
        }

        public AuthorizationException(ErrorType type)
        {
            ErrorType = type;
        }

        public AuthorizationException(ErrorType type, string message) : base(message)
        {
            ErrorType = type;
        }

        public AuthorizationException(ErrorType type, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorType = type;
        }
    }
}
