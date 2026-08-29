namespace IAS.Api;

internal static class WebHostCorsExtensions
{
    internal static string[] ResolveCorsOrigins(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var configured = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (configured is { Length: > 0 })
            return configured;

        if (environment.IsDevelopment())
            return ["http://localhost:5173", "http://127.0.0.1:5173"];

        return [];
    }

    internal static bool ShouldEnableSwagger(IConfiguration configuration, IWebHostEnvironment environment) =>
        configuration.GetValue("FeatureFlags:EnableSwagger", false)
        || environment.IsDevelopment();
}
