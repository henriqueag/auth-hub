using System.Text.Json.Serialization;
using AuthHub.Application.Dtos.Users;
using MediatR;

namespace AuthHub.Application.Commands.Users.Create;

public record CreateCommand : CreateUserRequest, IRequest<string>
{
    [JsonIgnore]
    public string Link { get; init; } = null!;
}