namespace AuthHub.Domain.Abstractions;

public record Error(string Code, string Message, IEnumerable<Error>? Errors = null);