using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.People;
using IAS.Domain.Unavailabilities;

namespace IAS.Api.IntegrationTests;

public sealed class UnavailabilitiesApiTests : IClassFixture<IasWebApplicationFactory>
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private readonly HttpClient _client;

    public UnavailabilitiesApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Crud_Indisponibilidade_RejeitaPeriodoSobreposto()
    {
        var personId = await CreatePersonAsync();

        var first = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            $"/api/v1/people/{personId}/unavailabilities",
            new CreateUnavailabilityRequest(
                new DateOnly(2026, 8, 1),
                new DateOnly(2026, 8, 10),
                UnavailabilityType.Vacation,
                "Férias"));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var overlap = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            $"/api/v1/people/{personId}/unavailabilities",
            new CreateUnavailabilityRequest(
                new DateOnly(2026, 8, 5),
                new DateOnly(2026,  8, 15),
                UnavailabilityType.Personal,
                null));
        Assert.Equal(HttpStatusCode.Conflict, overlap.StatusCode);

        var list = await SendWithTenant(
            TenantA,
            HttpMethod.Get,
            $"/api/v1/people/{personId}/unavailabilities");
        var page = await list.Content.ReadAsApiJsonAsync<PagedUnavailabilitiesResponse>();
        Assert.NotNull(page);
        Assert.Single(page.Items);
    }

    private async Task<Guid> CreatePersonAsync()
    {
        var response = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "João",
                "Dev",
                "Pleno",
                null,
                null,
                40,
                null,
                null,
                PersonStatus.Active));

        var person = await response.Content.ReadAsApiJsonAsync<PersonResponse>();
        Assert.NotNull(person);
        return person.Id;
    }

    private Task<HttpResponseMessage> SendWithTenant<T>(Guid tenantId, HttpMethod method, string url, T body) =>
        SendWithTenant(tenantId, new HttpRequestMessage(method, url) { Content = JsonContent.Create(body) });

    private Task<HttpResponseMessage> SendWithTenant(Guid tenantId, HttpMethod method, string url) =>
        SendWithTenant(tenantId, new HttpRequestMessage(method, url));

    private Task<HttpResponseMessage> SendWithTenant(Guid tenantId, HttpRequestMessage request)
    {
        request.Headers.Add(TenantMiddleware.TenantHeaderName, tenantId.ToString());
        return _client.SendAsync(request);
    }
}
