using IAS.Application.Identity;
using IAS.Domain.Identity;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.Identity;

public sealed class UserRepository(IasDbContext context) : IUserRepository
{
    public Task<bool> TenantExistsAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        context.Tenants
            .Where(t => t.DeletedAt == null && t.IsActive)
            .AnyAsync(t => t.Id == tenantId, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Users.Where(u => u.Email == email);
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<User> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Users.OrderBy(u => u.DisplayName);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await context.Users.AddAsync(user, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
