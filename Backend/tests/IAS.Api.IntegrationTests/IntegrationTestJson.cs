using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IAS.Api.IntegrationTests;

internal static class IntegrationTestJson
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
}

internal static class HttpContentExtensions
{
    public static Task<T?> ReadAsApiJsonAsync<T>(this HttpContent content) =>
        content.ReadFromJsonAsync<T>(IntegrationTestJson.Options);
}
