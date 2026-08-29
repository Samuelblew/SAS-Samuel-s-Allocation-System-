using IAS.Application.Common.Interfaces;

using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;
using IAS.Domain.AuditLogs;
using IAS.Domain.Matching;
using IAS.Domain.Clients;
using IAS.Domain.Identity;
using IAS.Domain.People;
using IAS.Domain.Projects;
using IAS.Domain.Skills;
using IAS.Domain.Tenancy;
using IAS.Domain.Unavailabilities;

using Microsoft.EntityFrameworkCore;



namespace IAS.Infrastructure.Persistence;



public class IasDbContext(

    DbContextOptions<IasDbContext> options,

    ITenantContext tenantContext,

    IAuditActorContext auditActorContext) : DbContext(options)

{

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<Person> People => Set<Person>();

    public DbSet<PersonSkill> PersonSkills => Set<PersonSkill>();

    public DbSet<Unavailability> Unavailabilities => Set<Unavailability>();

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<AllocationNeed> AllocationNeeds => Set<AllocationNeed>();

    public DbSet<Allocation> Allocations => Set<Allocation>();

    public DbSet<MatchingSuggestion> MatchingSuggestions => Set<MatchingSuggestion>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();



    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)

    {

        var pending = AuditChangeCapture.Capture(ChangeTracker, tenantContext);

        var count = await base.SaveChangesAsync(cancellationToken);



        if (pending.Count > 0)

        {

            foreach (var entry in pending)

                AuditLogs.Add(AuditChangeCapture.ToAuditLog(entry, auditActorContext));



            await base.SaveChangesAsync(cancellationToken);

        }



        return count;

    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IasDbContext).Assembly);



        modelBuilder.Entity<Skill>().HasQueryFilter(skill =>

            skill.TenantId == tenantContext.TenantId && skill.DeletedAt == null);



        modelBuilder.Entity<Person>().HasQueryFilter(person =>

            person.TenantId == tenantContext.TenantId && person.DeletedAt == null);



        modelBuilder.Entity<PersonSkill>().HasQueryFilter(personSkill =>

            personSkill.TenantId == tenantContext.TenantId && personSkill.DeletedAt == null);



        modelBuilder.Entity<Unavailability>().HasQueryFilter(unavailability =>

            unavailability.TenantId == tenantContext.TenantId && unavailability.DeletedAt == null);



        modelBuilder.Entity<Client>().HasQueryFilter(client =>

            client.TenantId == tenantContext.TenantId && client.DeletedAt == null);



        modelBuilder.Entity<Project>().HasQueryFilter(project =>

            project.TenantId == tenantContext.TenantId && project.DeletedAt == null);



        modelBuilder.Entity<AllocationNeed>().HasQueryFilter(need =>

            need.TenantId == tenantContext.TenantId && need.DeletedAt == null);



        modelBuilder.Entity<Allocation>().HasQueryFilter(allocation =>

            allocation.TenantId == tenantContext.TenantId && allocation.DeletedAt == null);



        modelBuilder.Entity<MatchingSuggestion>().HasQueryFilter(suggestion =>
            suggestion.TenantId == tenantContext.TenantId && suggestion.DeletedAt == null);

        modelBuilder.Entity<AuditLog>().HasQueryFilter(log =>
            log.TenantId == tenantContext.TenantId);

        modelBuilder.Entity<User>().HasQueryFilter(user =>
            user.TenantId == tenantContext.TenantId && user.DeletedAt == null);

        base.OnModelCreating(modelBuilder);

    }

}


