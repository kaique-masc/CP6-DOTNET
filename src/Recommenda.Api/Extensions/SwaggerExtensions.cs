using System.Reflection;
using Microsoft.OpenApi;
using Recommenda.Api.Swagger;

namespace Recommenda.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddRecommendaSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SwaggerSettings>(configuration.GetSection(SwaggerSettings.SectionName));
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile), includeControllerXmlComments: true);

            options.AddSecurityDefinition(AuthorizeOperationFilter.SchemeName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cole apenas o token retornado em POST /api/v2/auth/login (sem o prefixo Bearer)."
            });

            options.OperationFilter<AuthorizeOperationFilter>();
            options.OperationFilter<DeprecatedVersionOperationFilter>();
        });

        return services;
    }

    public static WebApplication UseRecommendaSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in app.DescribeApiVersions().OrderByDescending(d => d.ApiVersion))
            {
                var label = description.IsDeprecated
                    ? $"{description.GroupName.ToUpperInvariant()} (deprecada)"
                    : description.GroupName.ToUpperInvariant();

                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", label);
            }

            options.DocumentTitle = "Recommenda API";
        });

        return app;
    }
}
