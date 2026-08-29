using IAS.Domain.Identity;

namespace IAS.Application.Identity;

public interface IUserRepository
{
    Task<bool> TenantExistsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<User> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
