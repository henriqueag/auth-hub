namespace AuthHub.Infrastructure.Email;

public class SmtpOptions
{
    public required string Server { get; init; }
    public required int Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Sender { get; init; }
}