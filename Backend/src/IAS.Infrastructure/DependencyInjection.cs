using IAS.Application.Common.Interfaces;
using IAS.Application.AllocationNeeds;
using IAS.Application.Allocations;
using IAS.Application.AuditLogs;
using IAS.Application.Capacity;
using IAS.Application.Financial;
using IAS.Application.Matching;
using IAS.Application.Clients;
using IAS.Application.Identity;
using IAS.Application.People;
using IAS.Application.Projects;
using IAS.Application.Skills;
using IAS.Application.Tenancy;
using IAS.Application.Unavailabilities;
using IAS.Infrastructure.AllocationNeeds;
using IAS.Infrastructure.Allocations;
using IAS.Infrastructure.AuditLogs;
using IAS.Infrastructure.Capacity;
using IAS.Infrastructure.Financial;
using IAS.Infrastructure.Matching;
using IAS.Infrastructure.Clients;
using IAS.Infrastructure.Identity;
using IAS.Infrastructure.Persistence;
using IAS.Infrastructure.People;
using IAS.Infrastructure.Projects;
using IAS.Infrastructure.Skills;
using IAS.Infrastructure.Unavailabilities;
using IAS.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace IAS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<TenantContext>();
        services.AddScoped<IAuditActorContext>(sp => sp.GetRequiredService<AuditActorContext>());
        services.AddScoped<AuditActorContext>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IUnavailabilityRepository, UnavailabilityRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IAllocationNeedRepository, AllocationNeedRepository>();
        services.AddScoped<IAllocationRepository, AllocationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAllocationNeedStatusSync, AllocationNeedStatusSync>();
        services.AddScoped<ICapacityReadRepository, CapacityReadRepository>();
        services.AddScoped<IFinancialReadRepository, FinancialReadRepository>();
        services.AddScoped<IMatchingSuggestionRepository, MatchingSuggestionRepository>();

        if (bool.TryParse(configuration["Database:UseInMemory"], out var useInMemory) && useInMemory)
        {
            var inMemoryName = configuration["Database:InMemoryName"] ?? "ias_dev_inmemory";
            services.AddDbContext<IasDbContext>(options =>
                options.UseInMemoryDatabase(inMemoryName));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException(
                    "Connection string 'Default' não configurada. Use User Secrets em desenvolvimento.");

            var versionParts = (configuration["Database:MySqlVersion"] ?? "8.0.36")
                .Split('.', StringSplitOptions.RemoveEmptyEntries);
            var major = versionParts.Length > 0 ? int.Parse(versionParts[0]) : 8;
            var minor = versionParts.Length > 1 ? int.Parse(versionParts[1]) : 0;
            var patch = versionParts.Length > 2 ? int.Parse(versionParts[2]) : 36;
            var serverVersion = new MySqlServerVersion(new Version(major, minor, patch));

            services.AddDbContext<IasDbContext>(options =>
                options.UseMySql(connectionString, serverVersion));
        }

        return services;
    }
}
