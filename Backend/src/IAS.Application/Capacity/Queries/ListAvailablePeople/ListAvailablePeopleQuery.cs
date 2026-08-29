using MediatR;

namespace IAS.Application.Capacity.Queries.ListAvailablePeople;

public sealed record ListAvailablePeopleQuery(
    DateOnly From,
    DateOnly To,
    decimal MinAvailablePercent = 1) : IRequest<AvailablePeopleListDto>;

public sealed record AvailablePeopleListDto(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<AvailablePersonDto> People);

public sealed record AvailablePersonDto(
    Guid PersonId,
    string PersonName,
    decimal MinAvailablePercentInPeriod,
    decimal AvgAvailablePercent);
