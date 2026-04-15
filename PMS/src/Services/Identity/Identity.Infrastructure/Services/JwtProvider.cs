using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Models;
using Identity.Domain.Settings;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Services;

public class JwtProvider(IOptions<AuthJwtOptions> options) : IJwtProvider
{
    private readonly AuthJwtOptions _options = options.Value;

    public string GenerateAccessToken(AppUser appUser, IEnumerable<string> roles)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, appUser.Email!),
            new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var rsa = RSA.Create();
        rsa.ImportFromPem(_options.PrivateKey);
        var key = new RsaSecurityKey(rsa);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var expiryTime = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);
        return new(token, expiryTime);
    }
}
