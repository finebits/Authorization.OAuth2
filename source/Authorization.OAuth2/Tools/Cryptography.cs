﻿// ---------------------------------------------------------------------------- //
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
using System.Security.Cryptography;

namespace Finebits.Authorization.OAuth2.Tools
{
    internal static class Cryptography
    {
        public static byte[] GetHashSha256(byte[] input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(input);
            }
        }

        public static byte[] GetRandomValues(uint length)
        {
            byte[] buffer = new byte[length];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(buffer);
            }

            return buffer;
        }

        public static string ConvertToBase64UrlEncode(byte[] buffer, bool noPadding)
        {
            var base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");

            if (noPadding)
            {
                // Remove padding.
                base64 = base64.Replace("=", "");
            }

            return base64;
        }

        public static string GenerateRandomString(uint length)
        {
            var data = GetRandomValues(length);
            return ConvertToBase64UrlEncode(data, true);
        }
    }
}
