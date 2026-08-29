using IAS.Domain.Clients;

namespace IAS.Application.Clients;

internal static class ClientMapping
{
    public static ClientDto ToDto(this Client client) =>
        new(client.Id, client.Name, client.Notes, client.CreatedAt, client.UpdatedAt);

    public static ClientListItemDto ToListItemDto(this Client client) =>
        new(client.Id, client.Name, client.Projects.Count, client.CreatedAt);
}
