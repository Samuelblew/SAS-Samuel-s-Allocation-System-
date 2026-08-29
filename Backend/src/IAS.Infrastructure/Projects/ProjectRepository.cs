using IAS.Application.Projects;
using IAS.Domain.Projects;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.Projects;

public sealed class ProjectRepository(IasDbContext context) : IProjectRepository
{
    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Projects
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        context.Clients.AnyAsync(c => c.Id == clientId, cancellationToken);

    public async Task<(IReadOnlyList<Project> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        Guid? clientId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Projects.Include(p => p.Client).AsQueryable();

        if (clientId.HasValue)
            query = query.Where(p => p.ClientId == clientId.Value);

        query = query.OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default) =>
        await context.Projects.AddAsync(project, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
