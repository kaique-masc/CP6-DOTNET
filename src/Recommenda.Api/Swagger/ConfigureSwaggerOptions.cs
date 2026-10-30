using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Recommenda.Api.Swagger;

// Gera um documento Swagger para cada versão descoberta pelo ApiExplorer.
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly SwaggerSettings _settings;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<SwaggerSettings> settings)
    {
        _provider = provider;
        _settings = settings.Value;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateInfo(description));
    }

    private OpenApiInfo CreateInfo(ApiVersionDescription description)
    {
        var text = _settings.Description;
        if (description.IsDeprecated)
            text = "**VERSÃO DEPRECADA.** Esta versão continua disponível apenas para compatibilidade. " +
                   "Migre para a v2, que devolve a listagem de conteúdos paginada.<br/><br/>" + text;

        var info = new OpenApiInfo
        {
            Title = _settings.Title,
            Version = description.ApiVersion.ToString(),
            Description = text
        };

        if (!string.IsNullOrWhiteSpace(_settings.ContactName))
        {
            info.Contact = new OpenApiContact
            {
                Name = _settings.ContactName,
                Url = string.IsNullOrWhiteSpace(_settings.ContactUrl) ? null : new Uri(_settings.ContactUrl)
            };
        }

        return info;
    }
}
