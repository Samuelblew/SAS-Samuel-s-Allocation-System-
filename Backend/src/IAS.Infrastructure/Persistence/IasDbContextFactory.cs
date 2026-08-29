using System.Text.Json;
using IAS.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IAS.Infrastructure.Persistence;

/// <summary>
/// Factory para dotnet ef (design-time), sem tenant HTTP.
/// </summary>
public sealed class IasDbContextFactory : IDesignTimeDbContextFactory<IasDbContext>
{
    public IasDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IasDbContext>();
        var connectionString =
            Environment.GetEnvironmentVariable("IAS_CONNECTION_STRING")
            ?? ReadConnectionStringFromUserSecrets()
            ?? "Server=localhost;Port=3306;Database=ias_dev;User=root;Password=;";

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 46));
        optionsBuilder.UseMySql(connectionString, serverVersion);

        var tenantContext = new DesignTimeTenantContext();
        var auditActorContext = new DesignTimeAuditActorContext();
        return new IasDbContext(optionsBuilder.Options, tenantContext, auditActorContext);
    }

    private sealed class DesignTimeTenantContext : Application.Common.Interfaces.ITenantContext
    {
        public Guid TenantId => Guid.Empty;
        public bool IsResolved => false;
    }

    private sealed class DesignTimeAuditActorContext : Application.Common.Interfaces.IAuditActorContext
    {
        public string? ActorId => null;
        public string? ActorDisplayName => null;
        public bool IsResolved => false;
    }

    private static string? ReadConnectionStringFromUserSecrets()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft",
            "UserSecrets",
            "ias-api-dev",
            "secrets.json");

        if (!File.Exists(path))
            return null;

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.TryGetProperty("ConnectionStrings:Default", out var value)
            ? value.GetString()
            : null;
    }
}
