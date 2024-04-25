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

using System.Collections;

namespace Finebits.Authorization.OAuth2.Test.Data
{
    internal class AuthClientDataFixture
    {
        public static IEnumerable AuthClientFixtureData => CreateTestFixtureDataCollection(AuthClientCollection);
        public static IEnumerable RefreshableFixtureData => CreateTestFixtureDataCollection(RefreshableCollection);
        public static IEnumerable RevocableFixtureData => CreateTestFixtureDataCollection(RevocableCollection);
        public static IEnumerable ProfileReaderFixtureData => CreateTestFixtureDataCollection(ProfileReaderCollection);
        public static IEnumerable UserAvatarLoaderFixtureData => CreateTestFixtureDataCollection(UserAvatarLoaderCollection);

        public static IEnumerable AuthClientCaseData => CreateTestCaseDataCollection(AuthClientCollection);
        public static IEnumerable RefreshableCaseData => CreateTestCaseDataCollection(RefreshableCollection);
        public static IEnumerable NonRefreshableCaseData => CreateTestCaseDataCollection(AuthClientCollection.Except(RefreshableCollection));
        public static IEnumerable RevocableCaseData => CreateTestCaseDataCollection(RevocableCollection);
        public static IEnumerable IrrevocableCaseData => CreateTestCaseDataCollection(AuthClientCollection.Except(RevocableCollection));
        public static IEnumerable ProfileReaderCaseData => CreateTestCaseDataCollection(ProfileReaderCollection);
        public static IEnumerable NonProfileReaderCaseData => CreateTestCaseDataCollection(AuthClientCollection.Except(ProfileReaderCollection));
        public static IEnumerable UserAvatarLoaderCaseData => CreateTestCaseDataCollection(UserAvatarLoaderCollection);
        public static IEnumerable NonUserAvatarLoaderCaseData => CreateTestCaseDataCollection(AuthClientCollection.Except(UserAvatarLoaderCollection));

        private static IEnumerable<AuthClientType> AuthClientCollection
        {
            get
            {
                yield return AuthClientType.Google;
                yield return AuthClientType.Microsoft;
            }
        }

        private static IEnumerable<AuthClientType> RefreshableCollection
        {
            get
            {
                yield return AuthClientType.Google;
                yield return AuthClientType.Microsoft;
            }
        }

        private static IEnumerable<AuthClientType> RevocableCollection
        {
            get
            {
                yield return AuthClientType.Google;
            }
        }

        private static IEnumerable<AuthClientType> ProfileReaderCollection
        {
            get
            {
                yield return AuthClientType.Google;
                yield return AuthClientType.Microsoft;
            }
        }

        private static IEnumerable<AuthClientType> UserAvatarLoaderCollection
        {
            get
            {
                yield return AuthClientType.Google;
                yield return AuthClientType.Microsoft;
            }
        }

        private static IEnumerable CreateTestFixtureDataCollection(IEnumerable<AuthClientType> collection)
        {
            foreach (var item in collection)
            {
                yield return new TestFixtureData(item);
            }
        }

        private static IEnumerable CreateTestCaseDataCollection(IEnumerable<AuthClientType> collection)
        {
            foreach (var item in collection)
            {
                yield return new TestCaseData(item);
            }
        }
    }
}
