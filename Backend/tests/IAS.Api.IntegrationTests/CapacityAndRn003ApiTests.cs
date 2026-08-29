using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class CapacityAndRn003ApiTests : IClassFixture<IasWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CapacityAndRn003ApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Allocation_UpdatesAllocationNeedStatus_RN003()
    {
        var tenantId = await CreateTenantAsync();
        var (person, project) = await SeedPersonAndProjectAsync(tenantId);

        var needResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocation-needs",
            new CreateAllocationNeedRequest(
                project.Id,
                "Backend",
                null,
                [],
                [],
                50m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 12, 31),
                AllocationNeedUrgency.Medium,
                AllocationNeedCriticality.Medium,
                AllocationNeedStatus.Open));
        Assert.Equal(HttpStatusCode.Created, needResponse.StatusCode);
        var need = await needResponse.Content.ReadAsApiJsonAsync<AllocationNeedResponse>();
        Assert.NotNull(need);
        Assert.Equal(AllocationNeedStatus.Open, need.Status);

        var allocationResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocations",
            new CreateAllocationRequest(
                person.Id,
                project.Id,
                "Backend",
                50m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30),
                AllocationStatus.Confirmed,
                null));
        Assert.Equal(HttpStatusCode.Created, allocationResponse.StatusCode);

        var needAfter = await SendWithTenant(tenantId, HttpMethod.Get, $"/api/v1/allocation-needs/{need.Id}");
        var updated = await needAfter.Content.ReadAsApiJsonAsync<AllocationNeedResponse>();
        Assert.NotNull(updated);
        Assert.Equal(AllocationNeedStatus.Filled, updated.Status);
    }

    [Fact]
    public async Task PersonAvailability_ReturnsWeeklyBreakdown()
    {
        var tenantId = await CreateTenantAsync();
        var (person, project) = await SeedPersonAndProjectAsync(tenantId);

        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocations",
            new CreateAllocationRequest(
                person.Id,
                project.Id,
                "Backend",
                60m,
                new DateOnly(2026, 6, 2),
                new DateOnly(2026, 6, 20),
                AllocationStatus.Confirmed,
                null));

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            $"/api/v1/capacity/people/{person.Id}/availability?from=2026-06-01&to=2026-06-30");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var availability = await response.Content.ReadAsApiJsonAsync<PersonAvailabilityResponse>();
        Assert.NotNull(availability);
        Assert.NotEmpty(availability.Weeks);
        Assert.Contains(availability.Weeks, w => w.AllocatedPercent >= 60m);
    }

    [Fact]
    public async Task ProjectStaffingGaps_ShowsGapPercent()
    {
        var tenantId = await CreateTenantAsync();
        var (_, project) = await SeedPersonAndProjectAsync(tenantId);

        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocation-needs",
            new CreateAllocationNeedRequest(
                project.Id,
                "QA",
                null,
                [],
                [],
                100m,
                null,
                null,
                AllocationNeedUrgency.High,
                AllocationNeedCriticality.High,
                AllocationNeedStatus.Open));

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            $"/api/v1/capacity/projects/{project.Id}/staffing-gaps");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var gaps = await response.Content.ReadAsApiJsonAsync<ProjectStaffingGapsResponse>();
        Assert.NotNull(gaps);
        Assert.Single(gaps.Needs);
        Assert.Equal(100m, gaps.Needs[0].GapPercent);
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

    private async Task<(PersonResponse Person, ProjectResponse Project)> SeedPersonAndProjectAsync(Guid tenantId)
    {
        var personResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                $"Person {Guid.NewGuid():N}",
                "Dev",
                "Senior",
                null,
                null,
                40m,
                null,
                null,
                PersonStatus.Active));
        personResponse.EnsureSuccessStatusCode();
        var person = await personResponse.Content.ReadAsApiJsonAsync<PersonResponse>();

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
                ProjectPriority.Medium,
                null,
                null,
                null,
                null,
                null));
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>();

        return (person!, project!);
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
