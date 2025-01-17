namespace AuthHub.Infrastructure.Email;

public record TemplateInfo(string Subject, string FileName, IEnumerable<string> Variables)
{
    public string? Html { get; set; }
}