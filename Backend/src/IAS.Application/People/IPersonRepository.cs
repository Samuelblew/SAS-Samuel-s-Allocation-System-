using IAS.Domain.People;

namespace IAS.Application.People;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id, bool includeSkills, CancellationToken cancellationToken = default);
    Task<PersonSkill?> GetPersonSkillAsync(Guid personSkillId, CancellationToken cancellationToken = default);
    Task<bool> SkillExistsForPersonAsync(Guid personId, Guid skillId, Guid? excludePersonSkillId = null, CancellationToken cancellationToken = default);
    Task<bool> SkillCatalogExistsAsync(Guid skillId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Person> Items, int Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Person person, CancellationToken cancellationToken = default);
    Task AddPersonSkillAsync(PersonSkill personSkill, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
