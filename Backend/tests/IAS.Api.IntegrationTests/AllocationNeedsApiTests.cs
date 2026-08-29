using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class AllocationNeedsApiTests : IClassFixture<IasWebApplicationFactory>
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid TenantB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private readonly HttpClient _client;

    public AllocationNeedsApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Crud_NecessidadeAlocacao_IsolaTenant()
    {
        var skillResponse = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/skills",
            new CreateSkillRequest("C#", "Backend"));
        Assert.Equal(HttpStatusCode.Created, skillResponse.StatusCode);
        var skill = await skillResponse.Content.ReadAsApiJsonAsync<SkillResponse>();
        Assert.NotNull(skill);

        var clientResponse = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/clients",
            new CreateClientRequest("Cliente Need", null));
        var client = await clientResponse.Content.ReadAsApiJsonAsync<ClientResponse>();
        Assert.NotNull(client);

        var projectResponse = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/projects",
            new CreateProjectRequest(
                client.Id,
                "Projeto Need",
                ProjectStatus.InProgress,
                null,
                null,
                ProjectPriority.Medium,
                null,
                null,
                null,
                null,
                null));
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>();
        Assert.NotNull(project);

        var createNeed = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/allocation-needs",
            new CreateAllocationNeedRequest(
                project.Id,
                "Backend",
                "Senior",
                [skill.Id],
                [],
                80m,
                new DateOnly(2026, 7, 1),
                new DateOnly(2026, 12, 31),
                AllocationNeedUrgency.High,
                AllocationNeedCriticality.High,
                AllocationNeedStatus.Open));
        Assert.Equal(HttpStatusCode.Created, createNeed.StatusCode);

        var need = await createNeed.Content.ReadAsApiJsonAsync<AllocationNeedResponse>();
        Assert.NotNull(need);
        Assert.Equal("Projeto Need", need.ProjectName);
        Assert.Contains(skill.Id, need.RequiredSkillIds);

        var filterList = await SendWithTenant(
            TenantA,
            HttpMethod.Get,
            $"/api/v1/allocation-needs?projectId={project.Id}");
        var page = await filterList.Content.ReadAsApiJsonAsync<PagedAllocationNeedsResponse>();
        Assert.NotNull(page);
        Assert.Single(page.Items);

        var otherTenant = await SendWithTenant(TenantB, HttpMethod.Get, $"/api/v1/allocation-needs/{need.Id}");
        Assert.Equal(HttpStatusCode.NotFound, otherTenant.StatusCode);
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
