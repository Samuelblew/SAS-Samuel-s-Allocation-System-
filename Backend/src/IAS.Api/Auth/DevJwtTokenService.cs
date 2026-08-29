using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IAS.Api.Middleware;
using Microsoft.IdentityModel.Tokens;

namespace IAS.Api.Auth;

public sealed class DevJwtTokenService(IConfiguration configuration)
{
    public string CreateToken(Guid tenantId, Guid userId, string email, string displayName)
    {
        var secret = configuration["Auth:Jwt:Secret"]
            ?? throw new InvalidOperationException("Auth:Jwt:Secret não configurado.");
        var issuer = configuration["Auth:Jwt:Issuer"] ?? "IAS.Dev";
        var audience = configuration["Auth:Jwt:Audience"] ?? "IAS.Api";
        var hours = int.TryParse(configuration["Auth:Jwt:ExpirationHours"], out var h) ? h : 8;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtAuthenticationMiddleware.TenantClaimType, tenantId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtAuthenticationMiddleware.DisplayNameClaimType, displayName),
            new(ClaimTypes.Name, displayName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(hours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
