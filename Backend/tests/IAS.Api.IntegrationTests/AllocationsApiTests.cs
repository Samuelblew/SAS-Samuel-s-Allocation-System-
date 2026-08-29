using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Api.IntegrationTests;

public sealed class AllocationsApiTests : IClassFixture<IasWebApplicationFactory>
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid TenantB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private readonly HttpClient _client;

    public AllocationsApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Crud_Alocacao_IsolaTenant()
    {
        var (person, project) = await SeedPersonAndProjectAsync(TenantA);

        var create = await SendWithTenant(
            TenantA,
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
                "Alocação principal"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var allocation = await create.Content.ReadAsApiJsonAsync<AllocationResponse>();
        Assert.NotNull(allocation);
        Assert.Equal(person.Name, allocation.PersonName);
        Assert.Equal(project.Name, allocation.ProjectName);

        var list = await SendWithTenant(
            TenantA,
            HttpMethod.Get,
            $"/api/v1/allocations?personId={person.Id}");
        var page = await list.Content.ReadAsApiJsonAsync<PagedAllocationsResponse>();
        Assert.NotNull(page);
        Assert.Single(page.Items);

        var otherTenant = await SendWithTenant(TenantB, HttpMethod.Get, $"/api/v1/allocations/{allocation.Id}");
        Assert.Equal(HttpStatusCode.NotFound, otherTenant.StatusCode);
    }

    [Fact]
    public async Task Create_Superalocacao_Retorna409()
    {
        var (person, project) = await SeedPersonAndProjectAsync(TenantA);

        var first = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/allocations",
            new CreateAllocationRequest(
                person.Id,
                project.Id,
                "Backend",
                60m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30),
                AllocationStatus.Confirmed,
                null));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var clientResponse = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/clients",
            new CreateClientRequest("Outro cliente", null));
        var client = await clientResponse.Content.ReadAsApiJsonAsync<ClientResponse>();
        Assert.NotNull(client);

        var project2Response = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/projects",
            new CreateProjectRequest(
                client.Id,
                "Projeto B",
                ProjectStatus.InProgress,
                null,
                null,
                ProjectPriority.Medium,
                null,
                null,
                null,
                null,
                null));
        var project2 = await project2Response.Content.ReadAsApiJsonAsync<ProjectResponse>();
        Assert.NotNull(project2);

        var overload = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/allocations",
            new CreateAllocationRequest(
                person.Id,
                project2.Id,
                "Frontend",
                50m,
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30),
                AllocationStatus.Confirmed,
                null));
        Assert.Equal(HttpStatusCode.Conflict, overload.StatusCode);
    }

    private async Task<(PersonResponse Person, ProjectResponse Project)> SeedPersonAndProjectAsync(Guid tenantId)
    {
        var tag = Guid.NewGuid().ToString("N")[..8];

        var personResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                $"João Alocação {tag}",
                "Dev",
                "Pleno",
                null,
                null,
                40,
                null,
                null,
                PersonStatus.Active));
        Assert.Equal(HttpStatusCode.Created, personResponse.StatusCode);
        var person = await personResponse.Content.ReadAsApiJsonAsync<PersonResponse>()
            ?? throw new InvalidOperationException("Falha ao criar pessoa.");

        var clientResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/clients",
            new CreateClientRequest($"Cliente Alocação {tag}", null));
        Assert.Equal(HttpStatusCode.Created, clientResponse.StatusCode);
        var client = await clientResponse.Content.ReadAsApiJsonAsync<ClientResponse>()
            ?? throw new InvalidOperationException("Falha ao criar cliente.");

        var projectResponse = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/projects",
            new CreateProjectRequest(
                client.Id,
                $"Projeto A {tag}",
                ProjectStatus.InProgress,
                null,
                null,
                ProjectPriority.Medium,
                null,
                null,
                null,
                null,
                null));
        Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);
        var project = await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>()
            ?? throw new InvalidOperationException("Falha ao criar projeto.");

        return (person, project);
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
