using IAS.Domain.Unavailabilities;

namespace IAS.Application.Unavailabilities;

internal static class UnavailabilityMapping
{
    public static UnavailabilityDto ToDto(this Unavailability entity) =>
        new(
            entity.Id,
            entity.PersonId,
            entity.StartDate,
            entity.EndDate,
            entity.Type,
            entity.Notes,
            entity.CreatedAt,
            entity.UpdatedAt);
}
