using System.Text.Json.Serialization;
using AuthHub.Application.Dtos.Users;
using MediatR;

namespace AuthHub.Application.Commands.Users.Update;

public record UpdateCommand : BaseUserRequest, IRequest
{
    [JsonIgnore]
    public Guid UserId { get; init; }
    public bool Active { get; init; } = false;
    public IEnumerable<string>? RolesToAdd { get; set; }
    public IEnumerable<string>? RolesToRemove { get; set; }
}