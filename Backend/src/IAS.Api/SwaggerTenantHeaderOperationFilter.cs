using IAS.Api.Middleware;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IAS.Api;

public sealed class SwaggerTenantHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = TenantMiddleware.TenantHeaderName,
            In = ParameterLocation.Header,
            Required = true,
            Description = "GUID do tenant (obrigatório em dev)",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid" }
        });
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = AuditActorMiddleware.ActorIdHeaderName,
            In = ParameterLocation.Header,
            Required = false,
            Description = "Identificador do ator (opcional; gravado no audit log)",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }
        });
    }
}
