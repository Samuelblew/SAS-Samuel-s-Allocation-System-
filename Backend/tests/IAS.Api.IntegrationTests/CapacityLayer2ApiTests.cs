using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class CapacityLayer2ApiTests : IClassFixture<IasWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CapacityLayer2ApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Overview_ReturnsWeeklyStats()
    {
        var tenantId = await CreateTenantAsync();
        await SeedPersonAndProjectAsync(tenantId);

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            "/api/v1/capacity/overview?from=2026-06-01&to=2026-06-30");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var overview = await response.Content.ReadAsApiJsonAsync<CapacityOverviewResponse>();
        Assert.NotNull(overview);
        Assert.NotEmpty(overview.Weeks);
    }

    [Fact]
    public async Task Bench_ReturnsPeopleWithAvailability()
    {
        var tenantId = await CreateTenantAsync();
        await SeedPersonAndProjectAsync(tenantId);

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            "/api/v1/capacity/bench?from=2026-06-01&to=2026-06-30&minAvailablePercent=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var bench = await response.Content.ReadAsApiJsonAsync<BenchPeopleResponse>();
        Assert.NotNull(bench);
        Assert.NotEmpty(bench.People);
    }

    [Fact]
    public async Task UnderstaffedProjects_ListsProjectWithOpenNeed()
    {
        var tenantId = await CreateTenantAsync();
        var (_, project) = await SeedPersonAndProjectAsync(tenantId);

        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocation-needs",
            new CreateAllocationNeedRequest(
                project.Id,
                "DevOps",
                null,
                [],
                [],
                100m,
                null,
                null,
                AllocationNeedUrgency.High,
                AllocationNeedCriticality.High,
                AllocationNeedStatus.Open));

        var response = await SendWithTenant(tenantId, HttpMethod.Get, "/api/v1/projects/understaffed");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadAsApiJsonAsync<UnderstaffedProjectsResponse>();
        Assert.NotNull(list);
        Assert.Contains(list.Items, p => p.ProjectId == project.Id);
    }

    [Fact]
    public async Task SimulateProjectFeasibility_ReturnsResult()
    {
        var tenantId = await CreateTenantAsync();
        await SeedPersonAndProjectAsync(tenantId);

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/simulations/project-feasibility",
            new SimulateProjectFeasibilityRequest(
                new DateOnly(2026, 7, 1),
                3,
                [
                    new SimulatedNeedRequest("Backend", "Senior", [], 50, 1)
                ]));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadAsApiJsonAsync<ProjectFeasibilityResponse>();
        Assert.NotNull(result);
        Assert.Single(result.Roles);
        Assert.True(result.FeasibleAtDesiredStart);
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
                "Squad A",
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
