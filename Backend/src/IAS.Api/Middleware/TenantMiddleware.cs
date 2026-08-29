using IAS.Infrastructure.Tenancy;

namespace IAS.Api.Middleware;

public sealed class TenantMiddleware(RequestDelegate next)
{
    public const string TenantHeaderName = "X-Tenant-Id";

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            await next(context);
            return;
        }

        if (TenantBypassPaths.AllowsAnonymousTenant(context.Request.Path)
            || tenantContext.IsResolved)
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(TenantHeaderName, out var tenantHeader)
            || !Guid.TryParse(tenantHeader.FirstOrDefault(), out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc7807",
                title = "Tenant obrigatório",
                status = 400,
                detail = $"Informe o header '{TenantHeaderName}' com um GUID válido."
            });
            return;
        }

        tenantContext.SetTenant(tenantId);
        await next(context);
    }
}
