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
using System.Diagnostics.CodeAnalysis;

using Finebits.Authorization.OAuth2.Types;

namespace Finebits.Authorization.OAuth2.Test.CommonTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Unit Test Naming Conventions")]
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
internal class TypeTests
{
    private const string TokenType = nameof(TokenType);
    private const string AccessToken = nameof(AccessToken);
    private const string RefreshToken = nameof(RefreshToken);
    private const string IdToken = nameof(IdToken);
    private const string Scope = nameof(Scope);

    private const string FreshTokenType = nameof(FreshTokenType);
    private const string FreshAccessToken = nameof(FreshAccessToken);
    private const string FreshRefreshToken = nameof(FreshRefreshToken);
    private const string FreshIdToken = nameof(FreshIdToken);
    private const string FreshScope = nameof(FreshScope);

    private const string Empty = "";

    [Test]
    public void AuthenticationResult_Constructor_CorrectParam_Success()
    {
        const string Key1 = nameof(Key1);
        const string Key2 = nameof(Key2);
        const string Value1 = nameof(Key1);
        const string Value2 = nameof(Key2);

        NameValueCollection properties = new()
        {
            { Key1, Value1 },
            { Key2, Value2 },
        };

        AuthenticationResult? authResult = null;

        Assert.DoesNotThrow(() => authResult = new AuthenticationResult(properties));

        Assert.That(authResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authResult.Properties[Key1], Is.EqualTo(Value1));
            Assert.That(authResult.Properties[Key2], Is.EqualTo(Value2));
        });
    }

    [Test]
    public void AuthenticationResult_Constructor_NullParam_Exception()
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => new AuthenticationResult(null));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("properties"));
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType, TokenType)]
    [TestCase(AccessToken, RefreshToken, null, Credential.DefaultTokenType)]
    public void Credential_Constructor_CorrectParam_Success(string? accessToken, string? refreshToken, string? tokenType, string finalTokenType)
    {
        Credential? credential = null;

        Assert.DoesNotThrow(() => credential = new Credential(tokenType, accessToken, refreshToken));

        Assert.That(credential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(credential.AccessToken, Is.EqualTo(accessToken));
            Assert.That(credential.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(credential.TokenType, Is.EqualTo(finalTokenType));
        });
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType, TokenType)]
    [TestCase(AccessToken, RefreshToken, null, Credential.DefaultTokenType)]
    public void Credential_CopyConstructor_CorrectParam_Success(string? accessToken, string? refreshToken, string? tokenType, string finalTokenType)
    {
        Credential? credential = null;
        Credential? newCredential = null;

        Assert.DoesNotThrow(() =>
        {
            credential = new Credential(tokenType, accessToken, refreshToken);
            newCredential = new Credential(credential);
        });

        Assert.That(newCredential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(newCredential.AccessToken, Is.EqualTo(accessToken));
            Assert.That(newCredential.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(newCredential.TokenType, Is.EqualTo(finalTokenType));
        });
    }

    [Test]
    [TestCase(null, null, null, "accessToken")]
    [TestCase(null, RefreshToken, null, "accessToken")]
    [TestCase(null, null, TokenType, "accessToken")]
    [TestCase(null, RefreshToken, TokenType, "accessToken")]
    [TestCase(AccessToken, null, null, "refreshToken")]
    [TestCase(AccessToken, null, TokenType, "refreshToken")]
    public void Credential_Constructor_NullParam_Exception(string? accessToken, string? refreshToken, string? tokenType, string exceptionParamName)
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => new Credential(tokenType, accessToken, refreshToken));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo(exceptionParamName));
    }

    [Test]
    public void Credential_CopyConstructor_NullParam_Exception()
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => new Credential(null));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("other"));
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType,
              FreshAccessToken, FreshRefreshToken, FreshTokenType,
              FreshAccessToken, FreshRefreshToken, FreshTokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType,
              Empty, FreshRefreshToken, FreshTokenType,
              AccessToken, FreshRefreshToken, FreshTokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType,
             Empty, Empty, FreshTokenType,
             AccessToken, RefreshToken, FreshTokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType,
              Empty, FreshRefreshToken, Empty,
              AccessToken, FreshRefreshToken, TokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType,
              FreshAccessToken, Empty, FreshTokenType,
              FreshAccessToken, RefreshToken, FreshTokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType,
              FreshAccessToken, Empty, Empty,
              FreshAccessToken, RefreshToken, TokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType,
              FreshAccessToken, FreshRefreshToken, Empty,
              FreshAccessToken, FreshRefreshToken, TokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType,
              Empty, Empty, Empty,
              AccessToken, RefreshToken, TokenType)]
    public void Credential_Update_CorrectParam_Success(
        string accessToken, string refreshToken, string tokenType,
        string freshAccessToken, string freshRefreshToken, string freshTokenType,
        string finalAccessToken, string finalRefreshToken, string finalTokenType)
    {
        Credential? credential = null;
        Credential? fresh = null;

        Assert.DoesNotThrow(() =>
        {
            credential = new(tokenType, accessToken, refreshToken);
            fresh = new(freshTokenType, freshAccessToken, freshRefreshToken);

            credential.Update(fresh);
        });

        Assert.That(credential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(credential.AccessToken, Is.EqualTo(finalAccessToken));
            Assert.That(credential.RefreshToken, Is.EqualTo(finalRefreshToken));
            Assert.That(credential.TokenType, Is.EqualTo(finalTokenType));
        });
    }

    [Test]
    public void Credential_Update_NullParam_Exception()
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() =>
        {
            Credential credential = new(AccessToken, RefreshToken, TokenType);
            credential.Update(null);
        });

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("other"));
    }

    [Test]
    [TestCase(TokenType, AccessToken, RefreshToken, IdToken, 0, Scope, TokenType)]
    [TestCase(TokenType, AccessToken, RefreshToken, IdToken, TimeSpan.TicksPerDay, Scope, TokenType)]
    [TestCase(null, AccessToken, RefreshToken, IdToken, 0, Scope, Credential.DefaultTokenType)]
    [TestCase(null, AccessToken, RefreshToken, IdToken, TimeSpan.TicksPerHour, Scope, Credential.DefaultTokenType)]

    public void AuthorizationCredential_Constructor_CorrectParam_Success(string? tokenType, string accessToken, string refreshToken, string idToken, long expiresInTicks, string scope, string finalTokenType)
    {
        TimeSpan expiresIn = new(expiresInTicks);

        AuthCredential? credential = null;

        Assert.DoesNotThrow(() => credential = new AuthCredential(tokenType, accessToken, refreshToken, idToken, expiresIn, scope));

        Assert.That(credential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(credential.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(credential.AccessToken, Is.EqualTo(accessToken));
            Assert.That(credential.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(credential.IdToken, Is.EqualTo(IdToken));
            Assert.That(credential.ExpiresIn, Is.EqualTo(expiresIn));
            Assert.That(credential.Scope, Is.EqualTo(scope));
        });
    }

    [Test]
    [TestCase(TokenType, AccessToken, RefreshToken, IdToken, 0, Scope, TokenType)]
    [TestCase(TokenType, AccessToken, RefreshToken, IdToken, TimeSpan.TicksPerDay, Scope, TokenType)]
    [TestCase(null, AccessToken, RefreshToken, IdToken, 0, Scope, Credential.DefaultTokenType)]
    [TestCase(null, AccessToken, RefreshToken, IdToken, TimeSpan.TicksPerHour, Scope, Credential.DefaultTokenType)]

    public void AuthorizationCredential_CopyConstructor_CorrectParam_Success(string? tokenType, string accessToken, string refreshToken, string idToken, long expiresInTicks, string scope, string finalTokenType)
    {
        TimeSpan expiresIn = new(expiresInTicks);

        AuthCredential? credential = null;
        AuthCredential? newCredential = null;

        Assert.DoesNotThrow(() =>
        {
            credential = new AuthCredential(tokenType, accessToken, refreshToken, idToken, expiresIn, scope);
            newCredential = new AuthCredential(credential);
        });

        Assert.That(credential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(credential.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(credential.AccessToken, Is.EqualTo(accessToken));
            Assert.That(credential.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(credential.IdToken, Is.EqualTo(IdToken));
            Assert.That(credential.ExpiresIn, Is.EqualTo(expiresIn));
            Assert.That(credential.Scope, Is.EqualTo(scope));
        });
    }

    [Test]
    [TestCase(TokenType, AccessToken, RefreshToken, IdToken, TimeSpan.TicksPerDay, Scope,
              FreshTokenType, FreshAccessToken, FreshRefreshToken, FreshIdToken, TimeSpan.TicksPerHour, FreshScope,
              FreshTokenType, FreshAccessToken, FreshRefreshToken, FreshIdToken, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(TokenType, AccessToken, RefreshToken, IdToken, TimeSpan.TicksPerDay, Scope,
              Empty, Empty, Empty, Empty, 0, Empty,
              TokenType, AccessToken, RefreshToken, IdToken, 0, Scope)]
    public void AuthorizationCredential_Update_CorrectParam_Success(
        string tokenType, string accessToken, string refreshToken, string idToken, long expiresInTicks, string scope,
        string freshTokenType, string freshAccessToken, string freshRefreshToken, string freshIdToken, long freshExpiresInTicks, string freshScope,
        string finalTokenType, string finalAccessToken, string finalRefreshToken, string finalIdToken, long finalExpiresInTicks, string finalScope)
    {
        TimeSpan expiresIn = new(expiresInTicks);
        TimeSpan freshExpiresIn = new(freshExpiresInTicks);
        TimeSpan finalExpiresIn = new(finalExpiresInTicks);

        AuthCredential? credential = null;
        AuthCredential? fresh = null;

        Assert.DoesNotThrow(() =>
        {
            credential = new(tokenType, accessToken, refreshToken, idToken, expiresIn, scope);
            fresh = new(freshTokenType, freshAccessToken, freshRefreshToken, freshIdToken, freshExpiresIn, freshScope);

            credential.Update(fresh);
        });

        Assert.That(credential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(credential.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(credential.AccessToken, Is.EqualTo(finalAccessToken));
            Assert.That(credential.RefreshToken, Is.EqualTo(finalRefreshToken));
            Assert.That(credential.IdToken, Is.EqualTo(finalIdToken));
            Assert.That(credential.ExpiresIn, Is.EqualTo(finalExpiresIn));
            Assert.That(credential.Scope, Is.EqualTo(finalScope));
        });
    }

    [Test]
    [TestCase(null, null, null, null, null, "accessToken")]
    [TestCase(null, null, RefreshToken, null, null, "accessToken")]
    [TestCase(null, null, RefreshToken, IdToken, null, "accessToken")]
    [TestCase(null, null, RefreshToken, null, Scope, "accessToken")]
    [TestCase(null, null, RefreshToken, IdToken, Scope, "accessToken")]
    [TestCase(null, null, null, IdToken, Scope, "accessToken")]
    [TestCase(null, null, null, IdToken, null, "accessToken")]
    [TestCase(null, null, null, null, Scope, "accessToken")]
    [TestCase(TokenType, null, null, null, null, "accessToken")]
    [TestCase(TokenType, null, RefreshToken, null, null, "accessToken")]
    [TestCase(TokenType, null, RefreshToken, IdToken, null, "accessToken")]
    [TestCase(TokenType, null, RefreshToken, null, Scope, "accessToken")]
    [TestCase(TokenType, null, RefreshToken, IdToken, Scope, "accessToken")]
    [TestCase(TokenType, null, null, IdToken, Scope, "accessToken")]
    [TestCase(TokenType, null, null, IdToken, null, "accessToken")]
    [TestCase(TokenType, null, null, null, Scope, "accessToken")]
    [TestCase(null, AccessToken, null, null, null, "refreshToken")]
    [TestCase(null, AccessToken, null, IdToken, null, "refreshToken")]
    [TestCase(null, AccessToken, null, IdToken, Scope, "refreshToken")]
    [TestCase(null, AccessToken, null, null, Scope, "refreshToken")]
    [TestCase(TokenType, AccessToken, null, null, null, "refreshToken")]
    [TestCase(TokenType, AccessToken, null, IdToken, null, "refreshToken")]
    [TestCase(TokenType, AccessToken, null, IdToken, Scope, "refreshToken")]
    [TestCase(TokenType, AccessToken, null, null, Scope, "refreshToken")]
    [TestCase(null, AccessToken, RefreshToken, null, null, "idToken")]
    [TestCase(null, AccessToken, RefreshToken, null, Scope, "idToken")]
    [TestCase(TokenType, AccessToken, RefreshToken, null, null, "idToken")]
    [TestCase(TokenType, AccessToken, RefreshToken, null, Scope, "idToken")]
    [TestCase(null, AccessToken, RefreshToken, IdToken, null, "scope")]
    [TestCase(TokenType, AccessToken, RefreshToken, IdToken, null, "scope")]
    public void AuthorizationCredential_Constructor_NullParam_Exception(string? tokenType, string? accessToken, string? refreshToken, string? idToken, string? scope, string exceptionParamName)
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => new AuthCredential(tokenType, accessToken, refreshToken, idToken, default, scope));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo(exceptionParamName));
    }

    [Test]
    public void AuthorizationCredential_CopyConstructor_NullParam_Exception()
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => new AuthCredential(null));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("other"));
    }

    [Test]
    public void AuthorizationCredential_Update_NullParam_Exception()
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() =>
        {
            AuthCredential credential = new(TokenType, AccessToken, RefreshToken, IdToken, default, Scope);
            credential.Update(null);
        });

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("other"));
    }
}
