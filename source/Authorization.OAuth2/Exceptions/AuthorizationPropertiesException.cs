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
    public sealed class AuthorizationPropertiesException : AuthorizationException
    {
        private const string _emptyProperties = "Required properties are missing.";
        private const string _missingProperty = "A property is missing.";

        public string PropertyName { get; private set; }

        public AuthorizationPropertiesException()
            : base(ErrorType.InvalidResponse, _emptyProperties)
        { }

        public AuthorizationPropertiesException(string message)
            : base(ErrorType.InvalidResponse, message)
        { }

        public AuthorizationPropertiesException(string message, string propertyName)
            : base(ErrorType.InvalidResponse, GetPropertyMessage(message, propertyName))
        {
            PropertyName = propertyName;
        }

        public AuthorizationPropertiesException(Exception innerException)
            : base(ErrorType.InvalidResponse, _emptyProperties, innerException)
        { }

        public AuthorizationPropertiesException(string message, string propertyName, Exception innerException)
            : base(ErrorType.InvalidResponse, GetPropertyMessage(message, propertyName), innerException)
        { }

        public AuthorizationPropertiesException(string message, Exception innerException)
            : base(ErrorType.InvalidResponse, message, innerException)
        { }

        private static string GetPropertyMessage(string message, string propertyName)
        {
            return $"{message ?? _missingProperty} (Property '{propertyName}')";
        }
    }
}
