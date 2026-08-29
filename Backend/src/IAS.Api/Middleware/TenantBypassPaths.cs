namespace IAS.Api.Middleware;

internal static class TenantBypassPaths
{
    public static bool AllowsAnonymousTenant(PathString path)
    {
        if (path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase))
            return true;

        if (path.StartsWithSegments("/api/v1/health", StringComparison.OrdinalIgnoreCase))
            return true;

        if (path.StartsWithSegments("/api/v1/tenants", StringComparison.OrdinalIgnoreCase))
            return true;

        if (path.StartsWithSegments("/api/v1/auth/dev-token", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}
