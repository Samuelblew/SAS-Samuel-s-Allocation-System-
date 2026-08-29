using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;
using IAS.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace IAS.Api.IntegrationTests;

public sealed class AllocationViewsAndConflictsApiTests : IClassFixture<IasWebApplicationFactory>
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private readonly IasWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AllocationViewsAndConflictsApiTests(IasWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ViewsEConflitos_ProjetoPessoaESuperalocacao()
    {
        var tag = Guid.NewGuid().ToString("N")[..8];
        var person = await CreatePersonAsync(tag);
        var client = await CreateClientAsync(tag);
        var projectA = await CreateProjectAsync(client.Id, "Projeto View A");
        var projectB = await CreateProjectAsync(client.Id, "Projeto View B");

        await SeedSuperallocationAsync(person.Id, projectA.Id, projectB.Id);

        var projectPeople = await SendWithTenant(
            TenantA,
            HttpMethod.Get,
            $"/api/v1/projects/{projectA.Id}/people");
        Assert.Equal(HttpStatusCode.OK, projectPeople.StatusCode);
        var peopleView = await projectPeople.Content.ReadAsApiJsonAsync<ProjectPeopleViewResponse>();
        Assert.NotNull(peopleView);
        Assert.Single(peopleView.People);
        Assert.Single(peopleView.People[0].Allocations);

        var personProjects = await SendWithTenant(
            TenantA,
            HttpMethod.Get,
            $"/api/v1/people/{person.Id}/projects");
        var projectsView = await personProjects.Content.ReadAsApiJsonAsync<PersonProjectsViewResponse>();
        Assert.NotNull(projectsView);
        Assert.Equal(2, projectsView.Projects.Count);

        var conflicts = await SendWithTenant(
            TenantA,
            HttpMethod.Get,
            $"/api/v1/allocations/conflicts?personId={person.Id}");
        Assert.Equal(HttpStatusCode.OK, conflicts.StatusCode);
        var conflictList = await conflicts.Content.ReadAsApiJsonAsync<AllocationConflictsListResponse>();
        Assert.NotNull(conflictList);
        Assert.NotEmpty(conflictList.Items);
        Assert.True(conflictList.Items[0].TotalDedicationPercent > 100);
    }

    private async Task<PersonResponse> CreatePersonAsync(string tag)
    {
        var response = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                $"Pessoa View {tag}",
                "Dev",
                null,
                null,
                null,
                40,
                null,
                null,
                PersonStatus.Active));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.Content.ReadAsApiJsonAsync<PersonResponse>()
            ?? throw new InvalidOperationException("Falha ao criar pessoa.");
    }

    private async Task<ClientResponse> CreateClientAsync(string tag)
    {
        var clientResponse = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/clients",
            new CreateClientRequest($"Cliente View {tag}", null));
        Assert.Equal(HttpStatusCode.Created, clientResponse.StatusCode);
        return await clientResponse.Content.ReadAsApiJsonAsync<ClientResponse>()
            ?? throw new InvalidOperationException("Falha ao criar cliente.");
    }

    private async Task<ProjectResponse> CreateProjectAsync(Guid clientId, string projectName)
    {
        var projectResponse = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/projects",
            new CreateProjectRequest(
                clientId,
                projectName,
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
        return await projectResponse.Content.ReadAsApiJsonAsync<ProjectResponse>()
            ?? throw new InvalidOperationException("Falha ao criar projeto.");
    }

    private async Task SeedSuperallocationAsync(Guid personId, Guid projectAId, Guid projectBId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IasDbContext>();
        var now = DateTime.UtcNow;

        db.Allocations.AddRange(
            new Allocation
            {
                Id = Guid.NewGuid(),
                TenantId = TenantA,
                PersonId = personId,
                ProjectId = projectAId,
                Role = "Dev",
                DedicationPercent = 60,
                StartDate = new DateOnly(2026, 6, 1),
                EndDate = new DateOnly(2026, 6, 30),
                Status = AllocationStatus.Confirmed,
                CreatedAt = now
            },
            new Allocation
            {
                Id = Guid.NewGuid(),
                TenantId = TenantA,
                PersonId = personId,
                ProjectId = projectBId,
                Role = "Dev",
                DedicationPercent = 50,
                StartDate = new DateOnly(2026, 6, 1),
                EndDate = new DateOnly(2026, 6, 30),
                Status = AllocationStatus.Confirmed,
                CreatedAt = now
            });

        await db.SaveChangesAsync();
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
