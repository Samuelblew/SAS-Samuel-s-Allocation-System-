using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.Identity;

namespace IAS.Api.IntegrationTests;

public sealed class TenantsUsersAuthApiTests : IClassFixture<IasWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TenantsUsersAuthApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task CreateTenant_AndUser_ThenDevToken_Works()
    {
        var tenantResponse = await _client.PostAsJsonAsync("/api/v1/tenants", new CreateTenantRequest("Consultoria Alpha"));
        Assert.Equal(HttpStatusCode.Created, tenantResponse.StatusCode);
        var tenant = await tenantResponse.Content.ReadAsApiJsonAsync<TenantResponse>();
        Assert.NotNull(tenant);

        var userResponse = await SendWithTenant(
            tenant.Id,
            HttpMethod.Post,
            "/api/v1/users",
            new CreateUserRequest("ops@alpha.com", "Ops Lead", UserRole.Manager));
        Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);
        var user = await userResponse.Content.ReadAsApiJsonAsync<UserResponse>();
        Assert.NotNull(user);

        var tokenResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/dev-token",
            new DevTokenRequest(tenant.Id, user.Id));
        Assert.Equal(HttpStatusCode.OK, tokenResponse.StatusCode);
        var token = await tokenResponse.Content.ReadAsApiJsonAsync<DevTokenResponse>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
    }

    [Fact]
    public async Task JwtToken_ResolvesTenant_WithoutHeader()
    {
        var tenant = await CreateTenantAsync("JWT Tenant");
        var user = await CreateUserAsync(tenant.Id, "jwt@test.com", "JWT User");

        var tokenResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/dev-token",
            new DevTokenRequest(tenant.Id, user.Id));
        var token = await tokenResponse.Content.ReadAsApiJsonAsync<DevTokenResponse>();
        Assert.NotNull(token);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/skills");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", token.AccessToken);

        var skills = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, skills.StatusCode);
    }

    private async Task<TenantResponse> CreateTenantAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/tenants", new CreateTenantRequest(name));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadAsApiJsonAsync<TenantResponse>())!;
    }

    private async Task<UserResponse> CreateUserAsync(Guid tenantId, string email, string displayName)
    {
        var response = await SendWithTenant(
            tenantId,
            HttpMethod.Post,
            "/api/v1/users",
            new CreateUserRequest(email, displayName, UserRole.Member));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadAsApiJsonAsync<UserResponse>())!;
    }

    private Task<HttpResponseMessage> SendWithTenant<T>(Guid tenantId, HttpMethod method, string url, T body) =>
        SendWithTenant(tenantId, new HttpRequestMessage(method, url) { Content = JsonContent.Create(body) });

    private Task<HttpResponseMessage> SendWithTenant(Guid tenantId, HttpRequestMessage request)
    {
        request.Headers.Add(TenantMiddleware.TenantHeaderName, tenantId.ToString());
        return _client.SendAsync(request);
    }
}
