using IAS.Application.Common.Exceptions;
using IAS.Application.Unavailabilities;
using MediatR;

namespace IAS.Application.Unavailabilities.Queries.GetUnavailabilityById;

public sealed class GetUnavailabilityByIdQueryHandler(IUnavailabilityRepository repository)
    : IRequestHandler<GetUnavailabilityByIdQuery, UnavailabilityDto>
{
    public async Task<UnavailabilityDto> Handle(
        GetUnavailabilityByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Indisponibilidade '{request.Id}' não encontrada.");

        if (entity.PersonId != request.PersonId)
            throw new NotFoundException($"Indisponibilidade '{request.Id}' não pertence à pessoa informada.");

        return entity.ToDto();
    }
}
