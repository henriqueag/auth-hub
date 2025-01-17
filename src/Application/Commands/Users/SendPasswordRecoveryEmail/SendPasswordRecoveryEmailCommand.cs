using System.Text.Json.Serialization;
using MediatR;

namespace AuthHub.Application.Commands.Users.SendPasswordRecoveryEmail;

public record SendPasswordRecoveryEmailCommand(string? Email, [property: JsonIgnore] string Link) : IRequest;