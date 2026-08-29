using System.Net;
using System.Net.Http.Json;
using IAS.Api.Dtos;
using IAS.Api.Middleware;
using IAS.Domain.AuditLogs;

namespace IAS.Api.IntegrationTests;

public sealed class AuditLogsApiTests : IClassFixture<IasWebApplicationFactory>
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private const string ActorId = "user-dev-001";

    private readonly HttpClient _client;

    public AuditLogsApiTests(IasWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task CriarSkill_GeraAuditLogCreated()
    {
        var skillName = $"Skill Audit {Guid.NewGuid():N}";

        var create = await SendWithTenant(
            TenantA,
            HttpMethod.Post,
            "/api/v1/skills",
            new CreateSkillRequest(skillName, "Catálogo"),
            ActorId);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var skill = await create.Content.ReadAsApiJsonAsync<SkillResponse>();
        Assert.NotNull(skill);

        var list = await SendWithTenant(
            TenantA,
            HttpMethod.Get,
            $"/api/v1/audit-logs?entityType=Skill&entityId={skill.Id}&action=Created");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);

        var page = await list.Content.ReadAsApiJsonAsync<PagedAuditLogsResponse>();
        Assert.NotNull(page);
        Assert.Contains(page.Items, x =>
            x.EntityType == "Skill"
            && x.EntityId == skill.Id
            && x.Action == AuditAction.Created
            && x.ActorId == ActorId);
    }

    private Task<HttpResponseMessage> SendWithTenant<T>(
        Guid tenantId,
        HttpMethod method,
        string url,
        T body,
        string? actorId = null) =>
        SendWithTenant(tenantId, new HttpRequestMessage(method, url) { Content = JsonContent.Create(body) }, actorId);

    private Task<HttpResponseMessage> SendWithTenant(
        Guid tenantId,
        HttpMethod method,
        string url,
        string? actorId = null) =>
        SendWithTenant(tenantId, new HttpRequestMessage(method, url), actorId);

    private Task<HttpResponseMessage> SendWithTenant(
        Guid tenantId,
        HttpRequestMessage request,
        string? actorId = null)
    {
        request.Headers.Add(TenantMiddleware.TenantHeaderName, tenantId.ToString());
        if (!string.IsNullOrWhiteSpace(actorId))
            request.Headers.Add(AuditActorMiddleware.ActorIdHeaderName, actorId);

        return _client.SendAsync(request);
    }
}
