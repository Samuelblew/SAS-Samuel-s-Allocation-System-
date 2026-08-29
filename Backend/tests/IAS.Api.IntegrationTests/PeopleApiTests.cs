using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.People;

namespace IAS.Api.IntegrationTests;

public sealed class PeopleApiTests : IClassFixture<IasWebApplicationFactory>
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private readonly HttpClient _client;

    public PeopleApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Crud_PessoaComSkill_IsolaTenant()
    {
        var skillResponse = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/skills",
            new CreateSkillRequest("Java", "Backend"));
        var catalogSkill = await skillResponse.Content.ReadAsApiJsonAsync<SkillResponse>();
        Assert.NotNull(catalogSkill);

        var createPerson = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/people",
            new CreatePersonRequest(
                "Maria Silva",
                "Desenvolvedora",
                "Senior",
                null,
                12000m,
                40,
                "SP",
                "Delivery",
                PersonStatus.Active));
        Assert.Equal(HttpStatusCode.Created, createPerson.StatusCode);

        var person = await createPerson.Content.ReadAsApiJsonAsync<PersonResponse>();
        Assert.NotNull(person);

        var addSkill = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            $"/api/v1/people/{person.Id}/skills",
            new AddPersonSkillRequest(catalogSkill.Id, SkillProficiencyLevel.Expert, DateTime.UtcNow, "5 anos"));
        Assert.Equal(HttpStatusCode.Created, addSkill.StatusCode);

        var getPerson = await SendWithTenant(TenantA, HttpMethod.Get, $"/api/v1/people/{person.Id}");
        var loaded = await getPerson.Content.ReadAsApiJsonAsync<PersonResponse>();
        Assert.NotNull(loaded);
        Assert.Single(loaded.Skills);
        Assert.Equal("Java", loaded.Skills[0].SkillName);

        var otherTenantGet = await SendWithTenant(
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            HttpMethod.Get,
            $"/api/v1/people/{person.Id}");
        Assert.Equal(HttpStatusCode.NotFound, otherTenantGet.StatusCode);
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
