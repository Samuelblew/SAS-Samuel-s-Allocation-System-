using IAS.Application.Skills;
using IAS.Domain.Skills;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.Skills;

public sealed class SkillRepository(IasDbContext context) : ISkillRepository
{
    public Task<Skill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Skills.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Skills.Where(s => s.Name == name);

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Skill> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Skills.OrderBy(s => s.Name);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(Skill skill, CancellationToken cancellationToken = default) =>
        await context.Skills.AddAsync(skill, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
