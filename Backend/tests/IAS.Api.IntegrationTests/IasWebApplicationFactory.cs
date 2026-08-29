using IAS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace IAS.Api.IntegrationTests;

public sealed class IasWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"ias_test_{Guid.NewGuid():N}";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:UseInMemory"] = "true",
                ["Database:InMemoryName"] = _databaseName,
                ["Auth:Jwt:Secret"] = "ias-test-jwt-secret-for-integration-tests!!",
                ["Auth:Jwt:Issuer"] = "IAS.Dev",
                ["Auth:Jwt:Audience"] = "IAS.Api",
                ["Auth:Jwt:ExpirationHours"] = "1"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<IasDbContext>>();
            services.RemoveAll<IasDbContext>();

            services.AddDbContext<IasDbContext>((sp, options) =>
                options.UseInMemoryDatabase(_databaseName));
        });

        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IasDbContext>();
        db.Database.EnsureCreated();

        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseEnvironment("Testing");
}
