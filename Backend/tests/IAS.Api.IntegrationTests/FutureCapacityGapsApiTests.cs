using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class FutureCapacityGapsApiTests : IClassFixture<IasWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FutureCapacityGapsApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task FutureGaps_ReturnsTenantViewWithOpenNeed()
    {
        var tenantId = await CreateTenantAsync();

        var clientResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/clients",
            new CreateClientRequest($"Client {Guid.NewGuid():N}", null));
        clientResponse.EnsureSuccessStatusCode();
        var client = await clientResponse.Content.ReadAsApiJsonAsync<ClientResponse>();

        var projectResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/projects",
            new CreateProjectRequest(
                client!.Id,
                $"Project {Guid.NewGuid():N}",
                ProjectStatus.InProgress,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 12, 31),
                ProjectPriority.High,
                null,
                null,
                null,
                null,
                null));
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>();

        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Gap Dev",
                "Backend",
                "Senior",
                null,
                null,
                40m,
                null,
                null,
                PersonStatus.Active));

        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocation-needs",
            new CreateAllocationNeedRequest(
                project!.Id,
                "DevOps",
                null,
                [],
                [],
                80m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30),
                AllocationNeedUrgency.High,
                AllocationNeedCriticality.High,
                AllocationNeedStatus.Open));

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            "/api/v1/capacity/future-gaps?from=2026-06-01&to=2026-06-30");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var gaps = await response.Content.ReadAsApiJsonAsync<FutureCapacityGapsResponse>();
        Assert.NotNull(gaps);
        Assert.NotEmpty(gaps.OpenNeeds);
        Assert.NotEmpty(gaps.Weeks);
        Assert.Contains(gaps.OpenNeeds, n => n.Role == "DevOps" && n.GapPercent == 80m);
    }

    private async Task<Guid> CreateTenantAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/tenants",
            new CreateTenantRequest($"Tenant {Guid.NewGuid():N}"));
        response.EnsureSuccessStatusCode();
        var tenant = await response.Content.ReadAsApiJsonAsync<TenantResponse>();
        return tenant!.Id;
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
