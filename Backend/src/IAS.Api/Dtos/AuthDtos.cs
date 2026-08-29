namespace IAS.Api.Dtos;

public sealed record DevTokenRequest(Guid TenantId, Guid UserId);

public sealed record DevTokenResponse(
    string AccessToken,
    string TokenType,
    int ExpiresInHours);
