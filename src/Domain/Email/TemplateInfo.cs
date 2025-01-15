namespace AuthHub.Domain.Email;

public record TemplateInfo(string Subject, string Path, IEnumerable<string> Variables)
{
    public string? Html { get; set; }
}