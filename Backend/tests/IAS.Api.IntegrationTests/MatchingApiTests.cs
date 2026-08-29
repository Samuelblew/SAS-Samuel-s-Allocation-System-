using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Matching;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class MatchingApiTests : IClassFixture<IasWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MatchingApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetCandidates_ReturnsRankedListWithBreakdown()
    {
        var tenantId = await CreateTenantAsync();

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
                "Matcher Dev",
                "Backend",
                "Senior",
                80m,
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
                "Consulting",
                null,
                null));
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>();

        var needResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocation-needs",
            new CreateAllocationNeedRequest(
                project!.Id,
                "Backend",
                "Senior",
                [skill.Id],
                [],
                50m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 12, 31),
                AllocationNeedUrgency.High,
                AllocationNeedCriticality.High,
                AllocationNeedStatus.Open));
        needResponse.EnsureSuccessStatusCode();
        var need = await needResponse.Content.ReadAsApiJsonAsync<AllocationNeedResponse>();

        var candidatesResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            $"/api/v1/allocation-needs/{need!.Id}/candidates");

        Assert.Equal(HttpStatusCode.OK, candidatesResponse.StatusCode);
        var candidates = await candidatesResponse.Content.ReadAsApiJsonAsync<AllocationNeedCandidatesResponse>();
        Assert.NotNull(candidates);
        Assert.NotEmpty(candidates.Candidates);
        Assert.Contains(candidates.Candidates, c => c.PersonId == person.Id);
        Assert.True(candidates.Candidates[0].Breakdown.TotalScore > 0);
        Assert.True(candidates.Candidates[0].Breakdown.RequiredSkillsScore > 0);
    }

    [Fact]
    public async Task GetProjectMatchingCandidates_ReturnsCandidatesForAllOpenNeeds()
    {
        var tenantId = await CreateTenantAsync();

        var skillResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/skills",
            new CreateSkillRequest("Go", "Backend"));
        skillResponse.EnsureSuccessStatusCode();
        var skill = await skillResponse.Content.ReadAsApiJsonAsync<SkillResponse>();

        var personResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Batch Dev",
                "Backend",
                "Senior",
                80m,
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
                $"Batch Project {Guid.NewGuid():N}",
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

        foreach (var role in new[] { "Backend", "Frontend" })
        {
            await SendWithTenant(
                tenantId,
                HttpMethod.Post,
                "/api/v1/allocation-needs",
                new CreateAllocationNeedRequest(
                    project!.Id,
                    role,
                    "Senior",
                    role == "Backend" ? [skill.Id] : [],
                    [],
                    50m,
                    new DateOnly(2026, 6, 1),
                    new DateOnly(2026, 12, 31),
                    AllocationNeedUrgency.Medium,
                    AllocationNeedCriticality.Medium,
                    AllocationNeedStatus.Open));
        }

        var batchResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            $"/api/v1/projects/{project!.Id}/matching-candidates?maxResultsPerNeed=5");

        Assert.Equal(HttpStatusCode.OK, batchResponse.StatusCode);
        var batch = await batchResponse.Content.ReadAsApiJsonAsync<ProjectMatchingCandidatesResponse>();
        Assert.NotNull(batch);
        Assert.Equal(2, batch.Needs.Count);
        Assert.All(batch.Needs, n => Assert.NotEmpty(n.Candidates));
    }

    [Fact]
    public async Task RecordAndListMatchingSuggestions_PersistsHistory()
    {
        var tenantId = await CreateTenantAsync();

        var personResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "History Dev",
                "Backend",
                "Senior",
                80m,
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
                ProjectPriority.High,
                null,
                null,
                null,
                null,
                null));
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>();

        var needResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocation-needs",
            new CreateAllocationNeedRequest(
                project!.Id,
                "Backend",
                "Senior",
                [],
                [],
                50m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 12, 31),
                AllocationNeedUrgency.Medium,
                AllocationNeedCriticality.Medium,
                AllocationNeedStatus.Open));
        needResponse.EnsureSuccessStatusCode();
        var need = await needResponse.Content.ReadAsApiJsonAsync<AllocationNeedResponse>();

        var recordResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            $"/api/v1/allocation-needs/{need!.Id}/matching-suggestions",
            new RecordMatchingSuggestionRequest(
                person!.Id,
                MatchingSuggestionDecision.Accepted,
                72.5m,
                "Bom fit"));

        Assert.Equal(HttpStatusCode.Created, recordResponse.StatusCode);
        var recorded = await recordResponse.Content.ReadAsApiJsonAsync<MatchingSuggestionResponse>();
        Assert.NotNull(recorded);
        Assert.Equal(MatchingSuggestionDecision.Accepted, recorded.Decision);
        Assert.Equal(person.Id, recorded.PersonId);

        var listResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Get,
            $"/api/v1/allocation-needs/{need.Id}/matching-suggestions");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadAsApiJsonAsync<PagedMatchingSuggestionsResponse>();
        Assert.NotNull(list);
        Assert.Single(list.Items);
        Assert.Equal("History Dev", list.Items[0].PersonName);
    }

    [Fact]
    public async Task RecordMatchingSuggestion_WorksAfterExistingHistory()
    {
        var tenantId = await CreateTenantAsync();

        var personAResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "First Dev",
                "Backend",
                "Senior",
                80m,
                null,
                40m,
                null,
                null,
                PersonStatus.Active));
        personAResponse.EnsureSuccessStatusCode();
        var personA = await personAResponse.Content.ReadAsApiJsonAsync<PersonResponse>();

        var personBResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Second Dev",
                "Backend",
                "Pleno",
                70m,
                null,
                40m,
                null,
                null,
                PersonStatus.Active));
        personBResponse.EnsureSuccessStatusCode();
        var personB = await personBResponse.Content.ReadAsApiJsonAsync<PersonResponse>();

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

        var needResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/allocation-needs",
            new CreateAllocationNeedRequest(
                project!.Id,
                "Backend",
                "Senior",
                [],
                [],
                50m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 12, 31),
                AllocationNeedUrgency.Medium,
                AllocationNeedCriticality.Medium,
                AllocationNeedStatus.Open));
        needResponse.EnsureSuccessStatusCode();
        var need = await needResponse.Content.ReadAsApiJsonAsync<AllocationNeedResponse>();

        var firstResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            $"/api/v1/allocation-needs/{need!.Id}/matching-suggestions",
            new RecordMatchingSuggestionRequest(
                personA!.Id,
                MatchingSuggestionDecision.Accepted,
                70m,
                null));

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            $"/api/v1/allocation-needs/{need.Id}/matching-suggestions",
            new RecordMatchingSuggestionRequest(
                personB!.Id,
                MatchingSuggestionDecision.Rejected,
                55m,
                null));

        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
        var recorded = await secondResponse.Content.ReadAsApiJsonAsync<MatchingSuggestionResponse>();
        Assert.NotNull(recorded);
        Assert.Equal(personB.Id, recorded.PersonId);
        Assert.Equal(MatchingSuggestionDecision.Rejected, recorded.Decision);
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
