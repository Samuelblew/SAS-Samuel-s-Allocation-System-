using IAS.Application.People;
using IAS.Domain.People;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.People;

public sealed class PersonRepository(IasDbContext context) : IPersonRepository
{
    public Task<Person?> GetByIdAsync(Guid id, bool includeSkills, CancellationToken cancellationToken = default)
    {
        var query = context.People.AsQueryable();

        if (includeSkills)
            query = query
                .Include(p => p.Skills)
                .ThenInclude(ps => ps.Skill);

        return query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<PersonSkill?> GetPersonSkillAsync(Guid personSkillId, CancellationToken cancellationToken = default) =>
        context.PersonSkills
            .Include(ps => ps.Skill)
            .FirstOrDefaultAsync(ps => ps.Id == personSkillId, cancellationToken);

    public Task<bool> SkillExistsForPersonAsync(
        Guid personId,
        Guid skillId,
        Guid? excludePersonSkillId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.PersonSkills.Where(ps => ps.PersonId == personId && ps.SkillId == skillId);

        if (excludePersonSkillId.HasValue)
            query = query.Where(ps => ps.Id != excludePersonSkillId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> SkillCatalogExistsAsync(Guid skillId, CancellationToken cancellationToken = default) =>
        context.Skills.AnyAsync(s => s.Id == skillId, cancellationToken);

    public async Task<(IReadOnlyList<Person> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.People
            .Include(p => p.Skills)
            .OrderBy(p => p.Name);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(Person person, CancellationToken cancellationToken = default) =>
        await context.People.AddAsync(person, cancellationToken);

    public async Task AddPersonSkillAsync(PersonSkill personSkill, CancellationToken cancellationToken = default) =>
        await context.PersonSkills.AddAsync(personSkill, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
