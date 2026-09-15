using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AKCore.DataModel;
using AKCore.Models.Api.V1.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AKCore.Services.Api.V1.Auth;

public class MobileTokenService
{
    private readonly MobileAuthOptions _options;

    public MobileTokenService(IOptions<MobileAuthOptions> options)
    {
        _options = options.Value;
    }

    public string CreateAccessToken(AkUser user)
    {
        if (string.IsNullOrWhiteSpace(_options.SigningKey))
        {
            throw new InvalidOperationException(
                "MobileAuth signing key is not configured.");
        }

        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(
                JwtRegisteredClaimNames.UniqueName,
                user.UserName ?? ""),
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SigningKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.AccessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        return Base64UrlEncoder.Encode(
            RandomNumberGenerator.GetBytes(32));
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public DateTime GetRefreshTokenExpiry()
    {
        return DateTime.UtcNow.AddDays(_options.RefreshTokenDays);
    }
}