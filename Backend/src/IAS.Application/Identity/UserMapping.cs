using IAS.Domain.Identity;

namespace IAS.Application.Identity;

internal static class UserMapping
{
    public static UserDto ToDto(this User user) =>
        new(user.Id, user.Email, user.DisplayName, user.Role, user.IsActive, user.CreatedAt, user.UpdatedAt);
}
