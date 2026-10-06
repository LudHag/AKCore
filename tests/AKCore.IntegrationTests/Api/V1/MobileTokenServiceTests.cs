using System.IdentityModel.Tokens.Jwt;
using AKCore.DataModel;
using AKCore.Models.Api.V1.Auth;
using AKCore.Services.Api.V1.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AKCore.IntegrationTests.Services.Api.V1.Auth;

public class MobileTokenServiceTests
{
    private static MobileTokenService CreateService()
    {
        return new MobileTokenService(
            Options.Create(new MobileAuthOptions
            {
                Issuer = "AKCore.Tests",
                Audience = "AlteKamerer.Tests",
                SigningKey =
                    "0123456789abcdef0123456789abcdef",
                AccessTokenMinutes = 15,
                RefreshTokenDays = 30
            }));
    }

    [Fact]
    public void CreateRefreshToken_ReturnsDifferentRandomTokens()
    {
        var service = CreateService();

        var first = service.CreateRefreshToken();
        var second = service.CreateRefreshToken();

        Assert.NotEmpty(first);
        Assert.NotEmpty(second);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void HashRefreshToken_IsDeterministicSha256Hex()
    {
        var service = CreateService();

        var first = service.HashRefreshToken("refresh-token");
        var second = service.HashRefreshToken("refresh-token");

        Assert.Equal(first, second);
        Assert.Equal(64, first.Length);
        Assert.Matches("^[0-9a-f]{64}$", first);
    }

    [Fact]
    public void CreateAccessToken_ContainsExpectedIdentityClaims()
    {
        var service = CreateService();

        var user = new AkUser
        {
            Id = "user-123",
            UserName = "member"
        };

        var encoded = service.CreateAccessToken(user);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(encoded);

        Assert.Equal("AKCore.Tests", token.Issuer);
        Assert.Contains("AlteKamerer.Tests", token.Audiences);

        Assert.Equal(
            "user-123",
            token.Claims.Single(
                x => x.Type == JwtRegisteredClaimNames.Sub).Value);

        Assert.Equal(
            "member",
            token.Claims.Single(
                x => x.Type == JwtRegisteredClaimNames.UniqueName).Value);

        Assert.NotNull(
            token.Claims.SingleOrDefault(
                x => x.Type == JwtRegisteredClaimNames.Jti));
    }

    [Fact]
    public void CreateAccessToken_WithoutSigningKey_Throws()
    {
        var service = new MobileTokenService(
            Options.Create(new MobileAuthOptions()));

        var user = new AkUser
        {
            Id = "user-123",
            UserName = "member"
        };

        Assert.Throws<InvalidOperationException>(
            () => service.CreateAccessToken(user));
    }

    [Fact]
    public async Task MobileBearer_IsRegisteredWithExpectedTokenValidation()
    {
        await using var factory = new CustomWebApplicationFactory();

        using var scope = factory.Services.CreateScope();

        var schemes =
            scope.ServiceProvider.GetRequiredService<IAuthenticationSchemeProvider>();

        var scheme = await schemes.GetSchemeAsync("MobileBearer");

        Assert.NotNull(scheme);
        Assert.Equal(
            typeof(JwtBearerHandler),
            scheme.HandlerType);

        var options =
            scope.ServiceProvider
                .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
                .Get("MobileBearer");

        Assert.Equal(
            "AKCore.Tests",
            options.TokenValidationParameters.ValidIssuer);

        Assert.Equal(
            "AlteKamerer.Tests",
            options.TokenValidationParameters.ValidAudience);

        Assert.NotNull(
            options.TokenValidationParameters.IssuerSigningKey);

        Assert.True(
            options.TokenValidationParameters.ValidateIssuer);

        Assert.True(
            options.TokenValidationParameters.ValidateAudience);

        Assert.True(
            options.TokenValidationParameters.ValidateLifetime);

        Assert.True(
            options.TokenValidationParameters.ValidateIssuerSigningKey);
    }
}
