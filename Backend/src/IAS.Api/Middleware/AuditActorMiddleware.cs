using IAS.Infrastructure.Tenancy;

namespace IAS.Api.Middleware;

public sealed class AuditActorMiddleware(RequestDelegate next)
{
    public const string ActorIdHeaderName = "X-Actor-Id";

    public async Task InvokeAsync(HttpContext context, AuditActorContext auditActorContext)
    {
        if (!auditActorContext.IsResolved
            && context.Request.Headers.TryGetValue(ActorIdHeaderName, out var actorHeader))
        {
            var actorId = actorHeader.FirstOrDefault();
            auditActorContext.SetActor(actorId);
        }

        await next(context);
    }
}
