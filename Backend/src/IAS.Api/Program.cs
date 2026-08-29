using System.Text.Json.Serialization;
using IAS.Api;
using IAS.Api.Auth;
using IAS.Api.Middleware;
using IAS.Application;
using IAS.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<DevJwtTokenService>();

var corsOrigins = WebHostCorsExtensions.ResolveCorsOrigins(builder.Configuration, builder.Environment);
if (corsOrigins.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AppCors", policy =>
            policy.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "IAS API", Version = "v1" });
    options.OperationFilter<SwaggerTenantHeaderOperationFilter>();
});

var app = builder.Build();

app.UseForwardedHeaders();

if (corsOrigins.Length > 0)
    app.UseCors("AppCors");

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<JwtAuthenticationMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<AuditActorMiddleware>();

if (WebHostCorsExtensions.ShouldEnableSwagger(app.Configuration, app.Environment))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
