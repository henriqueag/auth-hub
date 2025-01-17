namespace AuthHub.Application.Dtos.Roles;

public record RoleRequest
{
    public string? Name { get; init; }
}