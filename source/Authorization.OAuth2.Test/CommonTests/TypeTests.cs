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
    private const string AccessToken = nameof(AccessToken);
    private const string RefreshToken = nameof(RefreshToken);
    private const string TokenType = nameof(TokenType);
    private const string Scope = nameof(Scope);

    private const string FreshAccessToken = nameof(FreshAccessToken);
    private const string FreshRefreshToken = nameof(FreshRefreshToken);
    private const string FreshTokenType = nameof(FreshTokenType);
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

        Assert.DoesNotThrow(() => credential = new Credential(accessToken, refreshToken, tokenType));

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
            credential = new Credential(accessToken, refreshToken, tokenType);
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
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => new Credential(accessToken, refreshToken, tokenType));

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
            credential = new(accessToken, refreshToken, tokenType);
            fresh = new(freshAccessToken, freshRefreshToken, freshTokenType);

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
    [TestCase(AccessToken, RefreshToken, TokenType, 0, Scope, TokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope, TokenType)]
    [TestCase(AccessToken, RefreshToken, null, 0, Scope, Credential.DefaultTokenType)]
    [TestCase(AccessToken, RefreshToken, null, TimeSpan.TicksPerHour, Scope, Credential.DefaultTokenType)]

    public void AuthorizationCredential_Constructor_CorrectParam_Success(string accessToken, string refreshToken, string? tokenType, long expiresInTicks, string scope, string finalTokenType)
    {
        TimeSpan expiresIn = new(expiresInTicks);

        AuthCredential? credential = null;

        Assert.DoesNotThrow(() => credential = new AuthCredential(accessToken, refreshToken, tokenType, expiresIn, scope));

        Assert.That(credential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(credential.AccessToken, Is.EqualTo(accessToken));
            Assert.That(credential.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(credential.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(credential.ExpiresIn, Is.EqualTo(expiresIn));
            Assert.That(credential.Scope, Is.EqualTo(scope));
        });
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType, 0, Scope, TokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope, TokenType)]
    [TestCase(AccessToken, RefreshToken, null, 0, Scope, Credential.DefaultTokenType)]
    [TestCase(AccessToken, RefreshToken, null, TimeSpan.TicksPerHour, Scope, Credential.DefaultTokenType)]

    public void AuthorizationCredential_CopyConstructor_CorrectParam_Success(string accessToken, string refreshToken, string? tokenType, long expiresInTicks, string scope, string finalTokenType)
    {
        TimeSpan expiresIn = new(expiresInTicks);

        AuthCredential? credential = null;
        AuthCredential? newCredential = null;

        Assert.DoesNotThrow(() =>
        {
            credential = new AuthCredential(accessToken, refreshToken, tokenType, expiresIn, scope);
            newCredential = new AuthCredential(credential);
        });

        Assert.That(credential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(credential.AccessToken, Is.EqualTo(accessToken));
            Assert.That(credential.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(credential.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(credential.ExpiresIn, Is.EqualTo(expiresIn));
            Assert.That(credential.Scope, Is.EqualTo(scope));
        });
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              FreshAccessToken, FreshRefreshToken, FreshTokenType, TimeSpan.TicksPerHour, FreshScope,
              FreshAccessToken, FreshRefreshToken, FreshTokenType, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              Empty, Empty, Empty, 0, Empty,
              AccessToken, RefreshToken, TokenType, 0, Scope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              Empty, FreshRefreshToken, FreshTokenType, TimeSpan.TicksPerHour, FreshScope,
              AccessToken, FreshRefreshToken, FreshTokenType, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              Empty, Empty, FreshTokenType, TimeSpan.TicksPerHour, FreshScope,
              AccessToken, RefreshToken, FreshTokenType, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              Empty, FreshRefreshToken, Empty, TimeSpan.TicksPerHour, FreshScope,
              AccessToken, FreshRefreshToken, TokenType, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              Empty, FreshRefreshToken, FreshTokenType, TimeSpan.TicksPerHour, Empty,
              AccessToken, FreshRefreshToken, FreshTokenType, TimeSpan.TicksPerHour, Scope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              Empty, Empty, Empty, TimeSpan.TicksPerHour, FreshScope,
              AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              Empty, Empty, FreshTokenType, TimeSpan.TicksPerHour, Empty,
              AccessToken, RefreshToken, FreshTokenType, TimeSpan.TicksPerHour, Scope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              Empty, FreshRefreshToken, Empty, TimeSpan.TicksPerHour, Empty,
              AccessToken, FreshRefreshToken, TokenType, TimeSpan.TicksPerHour, Scope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              FreshAccessToken, Empty, FreshTokenType, TimeSpan.TicksPerHour, FreshScope,
              FreshAccessToken, RefreshToken, FreshTokenType, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              FreshAccessToken, Empty, Empty, TimeSpan.TicksPerHour, FreshScope,
              FreshAccessToken, RefreshToken, TokenType, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              FreshAccessToken, Empty, FreshTokenType, TimeSpan.TicksPerHour, Empty,
              FreshAccessToken, RefreshToken, FreshTokenType, TimeSpan.TicksPerHour, Scope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              FreshAccessToken, Empty, Empty, TimeSpan.TicksPerHour, Empty,
              FreshAccessToken, RefreshToken, TokenType, TimeSpan.TicksPerHour, Scope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              FreshAccessToken, FreshRefreshToken, Empty, TimeSpan.TicksPerHour, FreshScope,
              FreshAccessToken, FreshRefreshToken, TokenType, TimeSpan.TicksPerHour, FreshScope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              FreshAccessToken, FreshRefreshToken, Empty, 0, Empty,
              FreshAccessToken, FreshRefreshToken, TokenType, 0, Scope)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope,
              FreshAccessToken, FreshRefreshToken, FreshTokenType, TimeSpan.TicksPerHour, Empty,
              FreshAccessToken, FreshRefreshToken, FreshTokenType, TimeSpan.TicksPerHour, Scope)]
    public void AuthorizationCredential_Update_CorrectParam_Success(
        string accessToken, string refreshToken, string tokenType, long expiresInTicks, string scope,
        string freshAccessToken, string freshRefreshToken, string freshTokenType, long freshExpiresInTicks, string freshScope,
        string finalAccessToken, string finalRefreshToken, string finalTokenType, long finalExpiresInTicks, string finalScope)
    {
        TimeSpan expiresIn = new(expiresInTicks);
        TimeSpan freshExpiresIn = new(freshExpiresInTicks);
        TimeSpan finalExpiresIn = new(finalExpiresInTicks);

        AuthCredential? credential = null;
        AuthCredential? fresh = null;

        Assert.DoesNotThrow(() =>
        {
            credential = new(accessToken, refreshToken, tokenType, expiresIn, scope);
            fresh = new(freshAccessToken, freshRefreshToken, freshTokenType, freshExpiresIn, freshScope);

            credential.Update(fresh);
        });

        Assert.That(credential, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(credential.AccessToken, Is.EqualTo(finalAccessToken));
            Assert.That(credential.RefreshToken, Is.EqualTo(finalRefreshToken));
            Assert.That(credential.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(credential.ExpiresIn, Is.EqualTo(finalExpiresIn));
            Assert.That(credential.Scope, Is.EqualTo(finalScope));
        });
    }

    [Test]
    [TestCase(null, null, null, null, "accessToken")]
    [TestCase(null, RefreshToken, null, null, "accessToken")]
    [TestCase(null, RefreshToken, TokenType, null, "accessToken")]
    [TestCase(null, RefreshToken, null, Scope, "accessToken")]
    [TestCase(null, RefreshToken, TokenType, Scope, "accessToken")]
    [TestCase(null, null, TokenType, null, "accessToken")]
    [TestCase(null, null, TokenType, Scope, "accessToken")]
    [TestCase(null, null, null, Scope, "accessToken")]
    [TestCase(AccessToken, null, null, null, "refreshToken")]
    [TestCase(AccessToken, null, TokenType, null, "refreshToken")]
    [TestCase(AccessToken, null, TokenType, Scope, "refreshToken")]
    [TestCase(AccessToken, null, null, Scope, "refreshToken")]
    [TestCase(AccessToken, RefreshToken, TokenType, null, "scope")]
    public void AuthorizationCredential_Constructor_NullParam_Exception(string? accessToken, string? refreshToken, string? tokenType, string? scope, string exceptionParamName)
    {
        ArgumentNullException? exception = Assert.Throws<ArgumentNullException>(() => new AuthCredential(accessToken, refreshToken, tokenType, default, scope));

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
            AuthCredential credential = new(AccessToken, RefreshToken, TokenType, default, Scope);
            credential.Update(null);
        });

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("other"));
    }
}
