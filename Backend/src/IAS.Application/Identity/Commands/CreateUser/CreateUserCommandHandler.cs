using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Domain.Identity;
using MediatR;

namespace IAS.Application.Identity.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository repository,
    ITenantContext tenantContext) : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        if (!await repository.TenantExistsAsync(tenantContext.TenantId, cancellationToken))
            throw new NotFoundException($"Tenant '{tenantContext.TenantId}' não encontrado.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await repository.ExistsByEmailAsync(email, cancellationToken: cancellationToken))
            throw new ConflictException($"Já existe um usuário com o e-mail '{email}'.");

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            Email = email,
            DisplayName = request.DisplayName.Trim(),
            Role = request.Role,
            IsActive = true,
            CreatedAt = now
        };

        await repository.AddAsync(user, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}
