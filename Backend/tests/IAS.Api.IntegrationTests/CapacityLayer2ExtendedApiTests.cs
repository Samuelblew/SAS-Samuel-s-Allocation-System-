using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class CapacityLayer2ExtendedApiTests : IClassFixture<IasWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CapacityLayer2ExtendedApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task PersonAvailability_IncludesEffectiveHours_RN004()
    {
        var tenantId = await CreateTenantAsync();
        var (person, project) = await SeedPersonWithSkillAsync(tenantId);

        await SendWithTenant(
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

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            $"/api/v1/capacity/people/{person.Id}/availability?from=2026-06-01&to=2026-06-30");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var availability = await response.Content.ReadAsApiJsonAsync<PersonAvailabilityResponse>();
        Assert.NotNull(availability);
        Assert.Equal(40m, availability.WeeklyCapacityHours);
        Assert.Contains(availability.Weeks, w => w.AllocatedHours == 20m);
    }

    [Fact]
    public async Task Overview_IncludesTotalHours_RN004()
    {
        var tenantId = await CreateTenantAsync();
        await SeedPersonWithSkillAsync(tenantId);

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            "/api/v1/capacity/overview?from=2026-06-01&to=2026-06-30");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var overview = await response.Content.ReadAsApiJsonAsync<CapacityOverviewResponse>();
        Assert.NotNull(overview);
        Assert.Contains(overview.Weeks, w => w.TotalCapacityHours > 0);
    }

    [Fact]
    public async Task SkillsOccupation_ReturnsAggregatedBySkill()
    {
        var tenantId = await CreateTenantAsync();
        await SeedPersonWithSkillAsync(tenantId);

        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            "/api/v1/capacity/skills-occupation?from=2026-06-01&to=2026-06-30");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var skills = await response.Content.ReadAsApiJsonAsync<SkillsOccupationResponse>();
        Assert.NotNull(skills);
        Assert.NotEmpty(skills.Skills);
        Assert.Contains(skills.Skills, s => s.SkillName == "Java");
    }

    private async Task<(PersonResponse Person, ProjectResponse Project)> SeedPersonWithSkillAsync(Guid tenantId)
    {
        var skillResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/skills",
            new CreateSkillRequest("Java", "Backend"));
        skillResponse.EnsureSuccessStatusCode();
        var skill = await skillResponse.Content.ReadAsApiJsonAsync<SkillResponse>();

        var personResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Skill Dev",
                "Backend",
                "Senior",
                null,
                null,
                40m,
                null,
                null,
                PersonStatus.Active));
        personResponse.EnsureSuccessStatusCode();
        var person = await personResponse.Content.ReadAsApiJsonAsync<PersonResponse>();

        await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            $"/api/v1/people/{person!.Id}/skills",
            new AddPersonSkillRequest(skill!.Id, SkillProficiencyLevel.Advanced, null, null));

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

        return (person, project!);
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
