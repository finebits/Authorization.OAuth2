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
        const string key1 = nameof(key1);
        const string key2 = nameof(key2);
        const string value1 = nameof(key1);
        const string value2 = nameof(key2);

        NameValueCollection properties = new()
        {
            { key1, value1 },
            { key2, value2 },
        };

        AuthenticationResult? authResult = null;

        Assert.DoesNotThrow(() => authResult = new AuthenticationResult(properties));

        Assert.That(authResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(authResult.Properties[key1], Is.EqualTo(value1));
            Assert.That(authResult.Properties[key2], Is.EqualTo(value2));
        });
    }

    [Test]
    public void AuthenticationResult_Constructor_NullParam_Exception()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new AuthenticationResult(null));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("properties"));
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType, TokenType)]
    [TestCase(AccessToken, RefreshToken, null, Token.DefaultTokenType)]
    public void Token_Constructor_CorrectParam_Success(string? accessToken, string? refreshToken, string? tokenType, string finalTokenType)
    {
        Token? token = null;

        Assert.DoesNotThrow(() => token = new Token(accessToken, refreshToken, tokenType));

        Assert.That(token, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(token.AccessToken, Is.EqualTo(accessToken));
            Assert.That(token.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(token.TokenType, Is.EqualTo(finalTokenType));
        });
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType, TokenType)]
    [TestCase(AccessToken, RefreshToken, null, Token.DefaultTokenType)]
    public void Token_CopyConstructor_CorrectParam_Success(string? accessToken, string? refreshToken, string? tokenType, string finalTokenType)
    {
        Token? token = null;
        Token? newToken = null;

        Assert.DoesNotThrow(() =>
        {
            token = new Token(accessToken, refreshToken, tokenType);
            newToken = new Token(token);
        });

        Assert.That(newToken, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(newToken.AccessToken, Is.EqualTo(accessToken));
            Assert.That(newToken.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(newToken.TokenType, Is.EqualTo(finalTokenType));
        });
    }

    [Test]
    [TestCase(null, null, null, "accessToken")]
    [TestCase(null, RefreshToken, null, "accessToken")]
    [TestCase(null, null, TokenType, "accessToken")]
    [TestCase(null, RefreshToken, TokenType, "accessToken")]
    [TestCase(AccessToken, null, null, "refreshToken")]
    [TestCase(AccessToken, null, TokenType, "refreshToken")]
    public void Token_Constructor_NullParam_Exception(string? accessToken, string? refreshToken, string? tokenType, string exceptionParamName)
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new Token(accessToken, refreshToken, tokenType));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo(exceptionParamName));
    }

    [Test]
    public void Token_CopyConstructor_NullParam_Exception()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new Token(null));

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
    public void Token_Update_CorrectParam_Success(
        string accessToken, string refreshToken, string tokenType,
        string freshAccessToken, string freshRefreshToken, string freshTokenType,
        string finalAccessToken, string finalRefreshToken, string finalTokenType)
    {
        Token? token = null;
        Token? fresh = null;

        Assert.DoesNotThrow(() =>
        {
            token = new(accessToken, refreshToken, tokenType);
            fresh = new(freshAccessToken, freshRefreshToken, freshTokenType);

            token.Update(fresh);
        });

        Assert.That(token, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(token.AccessToken, Is.EqualTo(finalAccessToken));
            Assert.That(token.RefreshToken, Is.EqualTo(finalRefreshToken));
            Assert.That(token.TokenType, Is.EqualTo(finalTokenType));
        });
    }

    [Test]
    public void Token_Update_NullParam_Exception()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            Token token = new(AccessToken, RefreshToken, TokenType);
            token.Update(null);
        });

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("other"));
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType, 0, Scope, TokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope, TokenType)]
    [TestCase(AccessToken, RefreshToken, null, 0, Scope, Token.DefaultTokenType)]
    [TestCase(AccessToken, RefreshToken, null, TimeSpan.TicksPerHour, Scope, Token.DefaultTokenType)]

    public void AuthorizationToken_Constructor_CorrectParam_Success(string accessToken, string refreshToken, string? tokenType, long expiresInTicks, string scope, string finalTokenType)
    {
        TimeSpan expiresIn = new(expiresInTicks);

        AuthorizationToken? token = null;

        Assert.DoesNotThrow(() => token = new AuthorizationToken(accessToken, refreshToken, tokenType, expiresIn, scope));

        Assert.That(token, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(token.AccessToken, Is.EqualTo(accessToken));
            Assert.That(token.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(token.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(token.ExpiresIn, Is.EqualTo(expiresIn));
            Assert.That(token.Scope, Is.EqualTo(scope));
        });
    }

    [Test]
    [TestCase(AccessToken, RefreshToken, TokenType, 0, Scope, TokenType)]
    [TestCase(AccessToken, RefreshToken, TokenType, TimeSpan.TicksPerDay, Scope, TokenType)]
    [TestCase(AccessToken, RefreshToken, null, 0, Scope, Token.DefaultTokenType)]
    [TestCase(AccessToken, RefreshToken, null, TimeSpan.TicksPerHour, Scope, Token.DefaultTokenType)]

    public void AuthorizationToken_CopyConstructor_CorrectParam_Success(string accessToken, string refreshToken, string? tokenType, long expiresInTicks, string scope, string finalTokenType)
    {
        TimeSpan expiresIn = new(expiresInTicks);

        AuthorizationToken? token = null;
        AuthorizationToken? newToken = null;

        Assert.DoesNotThrow(() =>
        {
            token = new AuthorizationToken(accessToken, refreshToken, tokenType, expiresIn, scope);
            newToken = new AuthorizationToken(token);
        });

        Assert.That(token, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(token.AccessToken, Is.EqualTo(accessToken));
            Assert.That(token.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(token.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(token.ExpiresIn, Is.EqualTo(expiresIn));
            Assert.That(token.Scope, Is.EqualTo(scope));
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
    public void AuthorizationToken_Update_CorrectParam_Success(
        string accessToken, string refreshToken, string tokenType, long expiresInTicks, string scope,
        string freshAccessToken, string freshRefreshToken, string freshTokenType, long freshExpiresInTicks, string freshScope,
        string finalAccessToken, string finalRefreshToken, string finalTokenType, long finalExpiresInTicks, string finalScope)
    {
        TimeSpan expiresIn = new(expiresInTicks);
        TimeSpan freshExpiresIn = new(freshExpiresInTicks);
        TimeSpan finalExpiresIn = new(finalExpiresInTicks);

        AuthorizationToken? token = null;
        AuthorizationToken? fresh = null;

        Assert.DoesNotThrow(() =>
        {
            token = new(accessToken, refreshToken, tokenType, expiresIn, scope);
            fresh = new(freshAccessToken, freshRefreshToken, freshTokenType, freshExpiresIn, freshScope);

            token.Update(fresh);
        });

        Assert.That(token, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(token.AccessToken, Is.EqualTo(finalAccessToken));
            Assert.That(token.RefreshToken, Is.EqualTo(finalRefreshToken));
            Assert.That(token.TokenType, Is.EqualTo(finalTokenType));
            Assert.That(token.ExpiresIn, Is.EqualTo(finalExpiresIn));
            Assert.That(token.Scope, Is.EqualTo(finalScope));
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
    public void AuthorizationToken_Constructor_NullParam_Exception(string? accessToken, string? refreshToken, string? tokenType, string? scope, string exceptionParamName)
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new AuthorizationToken(accessToken, refreshToken, tokenType, default, scope));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo(exceptionParamName));
    }

    [Test]
    public void AuthorizationToken_CopyConstructor_NullParam_Exception()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new AuthorizationToken(null));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("other"));
    }

    [Test]
    public void AuthorizationToken_Update_NullParam_Exception()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            AuthorizationToken token = new(AccessToken, RefreshToken, TokenType, default, Scope);
            token.Update(null);
        });

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.ParamName, Is.EqualTo("other"));
    }
}
