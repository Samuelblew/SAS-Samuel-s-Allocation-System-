using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IAS.Infrastructure.Tenancy;
using Microsoft.IdentityModel.Tokens;

namespace IAS.Api.Middleware;

public sealed class JwtAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public const string TenantClaimType = "tenant_id";
    public const string DisplayNameClaimType = "display_name";

    public async Task InvokeAsync(
        HttpContext context,
        TenantContext tenantContext,
        AuditActorContext auditActorContext)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(authHeader)
            && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            if (TryValidate(token, out var principal))
            {
                var tenantClaim = principal!.FindFirst(TenantClaimType)?.Value;
                if (Guid.TryParse(tenantClaim, out var tenantId))
                    tenantContext.SetTenant(tenantId);

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var displayName = principal.FindFirst(DisplayNameClaimType)?.Value
                    ?? principal.FindFirst(ClaimTypes.Name)?.Value;

                if (!string.IsNullOrWhiteSpace(userId))
                    auditActorContext.SetActor(userId, displayName);
            }
        }

        await next(context);
    }

    private bool TryValidate(string token, out ClaimsPrincipal? principal)
    {
        principal = null;
        var secret = configuration["Auth:Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
            return false;

        var issuer = configuration["Auth:Jwt:Issuer"] ?? "IAS.Dev";
        var audience = configuration["Auth:Jwt:Audience"] ?? "IAS.Api";

        try
        {
            var handler = new JwtSecurityTokenHandler();
            principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
