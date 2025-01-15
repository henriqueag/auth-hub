using System.Collections.Concurrent;
using System.Text.Json;

namespace SampleSecurityProvider.Email;

public class EmailTemplateReader : IEmailTemplateReader
{
    private readonly Lazy<ConcurrentDictionary<string, TemplateInfo>> _templates = new(LoadTemplates);

    public TemplateInfo GetTemplate(string templateName, IDictionary<string, string> variables)
    {
        if (!_templates.Value.TryGetValue(templateName, out var templateInfo))
        {
            throw new ArgumentException($"Template de email {templateName} não encontrado.");
        }

        var htmlContent = templateInfo.Html!;
        
        foreach (var variable in templateInfo.Variables)
        {
            if (!variables.TryGetValue(variable, out var inputVariable))
            {
                throw new ArgumentException($"Variável obrigatória '{variable}' não fornecida para o template '{templateName}'.");
            }
            
            htmlContent = htmlContent.Replace($"{{{variable}}}", inputVariable);
        }
        
        templateInfo.Html = htmlContent;
        
        return templateInfo;
    }
    
    private static ConcurrentDictionary<string, TemplateInfo> LoadTemplates()
    {
        var templates = new ConcurrentDictionary<string, TemplateInfo>();
        
        var assembly = typeof(EmailTemplateReader).Assembly;
        var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Email.Templates.TemplatesMap.json")!;
        var templatesMap = JsonSerializer.Deserialize<IDictionary<string, TemplateInfo>>(stream)!;
        
        foreach (var (key, value) in templatesMap)
        {
            if (!File.Exists(value.Path))
            {
                throw new FileNotFoundException($"O arquivo de template de email não foi encontrado no diretório {value.Path}");
            }
            
            value.Html = File.ReadAllText(value.Path);
            templates.TryAdd(key, value);
        }

        return templates;
    }
}

public record TemplateInfo(string Subject, string Path, IEnumerable<string> Variables)
{
    public string? Html { get; set; }
}