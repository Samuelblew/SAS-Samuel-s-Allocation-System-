using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IAS.Infrastructure.Persistence;

internal static class JsonListGuidConverter
{
    private static readonly JsonSerializerOptions Options = new();

    public static ValueConverter<List<Guid>, string> Create() =>
        new(
            v => JsonSerializer.Serialize(v, Options),
            v => JsonSerializer.Deserialize<List<Guid>>(v, Options) ?? new List<Guid>());

    public static ValueComparer<List<Guid>> CreateComparer() =>
        new(
            (a, b) => a!.SequenceEqual(b!),
            v => v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            v => v.ToList());
}
