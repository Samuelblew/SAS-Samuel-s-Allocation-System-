using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class FinancialApiTests : IClassFixture<IasWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FinancialApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetProjectFinancials_ReturnsCostAndMargin()
    {
        var tenantId = await CreateTenantAsync();

        var personResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Finance Dev",
                "Backend",
                "Senior",
                100m,
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
                new DateOnly(2026, 6, 30),
                ProjectPriority.High,
                null,
                100_000m,
                null,
                null,
                null));
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>();

        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocations",
            new CreateAllocationRequest(
                person!.Id,
                project!.Id,
                "Backend",
                50m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30),
                AllocationStatus.Confirmed,
                null));

        var financialsResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            $"/api/v1/projects/{project.Id}/financials?from=2026-06-01&to=2026-06-30");

        Assert.Equal(HttpStatusCode.OK, financialsResponse.StatusCode);
        var financials = await financialsResponse.Content.ReadAsApiJsonAsync<ProjectFinancialsResponse>();
        Assert.NotNull(financials);
        Assert.True(financials.TotalCost > 0);
        Assert.NotNull(financials.MarginPercent);
        Assert.True(financials.MarginPercent > 0);
        Assert.Single(financials.Allocations);
        Assert.False(financials.IsLowMarginAlert);
    }

    [Fact]
    public async Task GetBenchCost_ReturnsBenchPeopleCost()
    {
        var tenantId = await CreateTenantAsync();
        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Bench Only",
                "Backend",
                "Senior",
                80m,
                null,
                40m,
                null,
                null,
                PersonStatus.Active));

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            "/api/v1/financials/bench?from=2026-06-01&to=2026-06-30&minAvailablePercent=50");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var bench = await response.Content.ReadAsApiJsonAsync<BenchCostResponse>();
        Assert.NotNull(bench);
        Assert.NotEmpty(bench.People);
        Assert.True(bench.TotalBenchCost > 0);
    }

    [Fact]
    public async Task SimulateAllocationMargin_ReturnsProjectedMargin()
    {
        var tenantId = await CreateTenantAsync();

        var personResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Margin Sim Dev",
                "Backend",
                "Senior",
                100m,
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
                new DateOnly(2026, 6, 30),
                ProjectPriority.High,
                null,
                50_000m,
                null,
                null,
                null));
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>();

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/simulations/allocation-margin",
            new SimulateAllocationMarginRequest(
                project!.Id,
                person!.Id,
                "Backend",
                50m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30),
                15m));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sim = await response.Content.ReadAsApiJsonAsync<AllocationMarginSimulationResponse>();
        Assert.NotNull(sim);
        Assert.True(sim.SimulatedAdditionalCost > 0);
        Assert.True(sim.MarginDeltaAmount < 0);
    }

    [Fact]
    public async Task GetProfitabilityByClient_GroupsProjects()
    {
        var tenantId = await CreateTenantAsync();
        await SeedLowMarginProjectAsync(tenantId);

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            "/api/v1/financials/profitability?from=2026-06-01&to=2026-06-30&groupBy=Client");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profitability = await response.Content.ReadAsApiJsonAsync<ProfitabilityResponse>();
        Assert.NotNull(profitability);
        Assert.Equal("Client", profitability.GroupBy);
        Assert.NotEmpty(profitability.Groups);
        Assert.True(profitability.Groups[0].ProjectCount >= 1);
        Assert.True(profitability.Groups[0].TotalCost > 0);
    }

    [Fact]
    public async Task GetFinancialOverview_ReturnsProjectsAndAlerts()
    {
        var tenantId = await CreateTenantAsync();
        await SeedLowMarginProjectAsync(tenantId);

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            "/api/v1/financials/overview?from=2026-06-01&to=2026-06-30&marginAlertThreshold=50");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var overview = await response.Content.ReadAsApiJsonAsync<FinancialOverviewResponse>();
        Assert.NotNull(overview);
        Assert.NotEmpty(overview.Projects);
        Assert.NotEmpty(overview.LowMarginAlerts);
    }

    private async Task SeedLowMarginProjectAsync(Guid tenantId)
    {
        var personResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Expensive Dev",
                "Backend",
                "Senior",
                200m,
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
                $"Low Margin {Guid.NewGuid():N}",
                ProjectStatus.InProgress,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30),
                ProjectPriority.High,
                null,
                10_000m,
                null,
                null,
                null));
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>();

        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocations",
            new CreateAllocationRequest(
                person!.Id,
                project!.Id,
                "Backend",
                100m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30),
                AllocationStatus.Confirmed,
                null));
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
