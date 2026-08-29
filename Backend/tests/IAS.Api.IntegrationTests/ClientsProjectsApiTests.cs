using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class ClientsProjectsApiTests : IClassFixture<IasWebApplicationFactory>
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid TenantB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private readonly HttpClient _client;

    public ClientsProjectsApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Crud_ClienteEProjeto_IsolaTenant()
    {
        var createClient = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/clients",
            new CreateClientRequest("Acme Corp", "Cliente principal"));
        Assert.Equal(HttpStatusCode.Created, createClient.StatusCode);

        var client = await createClient.Content.ReadAsApiJsonAsync<ClientResponse>();
        Assert.NotNull(client);

        var createProject = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/projects",
            new CreateProjectRequest(
                client.Id,
                "Portal IAS",
                ProjectStatus.InProgress,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 12, 31),
                ProjectPriority.High,
                500000m,
                750000m,
                "Consultoria",
                "Comercial A",
                "Delivery B"));
        Assert.Equal(HttpStatusCode.Created, createProject.StatusCode);

        var project = await createProject.Content.ReadAsApiJsonAsync<ProjectResponse>();
        Assert.NotNull(project);
        Assert.Equal("Acme Corp", project.ClientName);

        var filterList = await SendWithTenant(
            TenantA,
            HttpMethod.Get,
            $"/api/v1/projects?clientId={client.Id}");
        var projectsPage = await filterList.Content.ReadAsApiJsonAsync<PagedProjectsResponse>();
        Assert.NotNull(projectsPage);
        Assert.Single(projectsPage.Items);

        var otherTenant = await SendWithTenant(TenantB, HttpMethod.Get, $"/api/v1/projects/{project.Id}");
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
