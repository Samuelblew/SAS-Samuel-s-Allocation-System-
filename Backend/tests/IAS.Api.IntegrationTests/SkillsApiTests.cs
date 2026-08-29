using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;

namespace IAS.Api.IntegrationTests;

public sealed class SkillsApiTests : IClassFixture<IasWebApplicationFactory>
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid TenantB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private readonly HttpClient _client;

    public SkillsApiTests(IasWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_SemTenantHeader_Retorna400()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/skills", new CreateSkillRequest("Java", "Backend"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Crud_ComTenant_IsolaDadosEntreTenants()
    {
        var createRequest = new CreateSkillRequest("TypeScript", "Frontend");

        var createResponse = await SendWithTenant(TenantA, HttpMethod.Post, "/api/v1/skills", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadAsApiJsonAsync<SkillResponse>();
        Assert.NotNull(created);
        Assert.Equal("TypeScript", created.Name);

        var getOtherTenant = await SendWithTenant(TenantB, HttpMethod.Get, $"/api/v1/skills/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getOtherTenant.StatusCode);

        var updateRequest = new UpdateSkillRequest("TypeScript 5", "Frontend");
        var updateResponse = await SendWithTenant(
            TenantA,
            HttpMethod.Put,
            $"/api/v1/skills/{created.Id}",
            updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var listResponse = await SendWithTenant(TenantA, HttpMethod.Get, "/api/v1/skills?page=1&pageSize=10");
        var list = await listResponse.Content.ReadAsApiJsonAsync<PagedSkillsResponse>();
        Assert.NotNull(list);
        Assert.Single(list.Items);
        Assert.Equal("TypeScript 5", list.Items[0].Name);

        var deleteResponse = await SendWithTenant(TenantA, HttpMethod.Delete, $"/api/v1/skills/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDelete = await SendWithTenant(TenantA, HttpMethod.Get, $"/api/v1/skills/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
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
