using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Recommenda.Api.HealthChecks;
using Recommenda.Infrastructure.Persistence;

namespace Recommenda.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddRecommendaHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API em execução"), tags: ["live"])
            .AddDbContextCheck<RecommendaContext>("mysql", HealthStatus.Unhealthy, tags: ["db", "mysql"])
            .AddMongoDb(
                clientFactory: sp => sp.GetRequiredService<IMongoClient>(),
                name: "mongodb",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "mongodb"],
                timeout: TimeSpan.FromSeconds(5));

        var externalUrl = configuration["HealthChecks:ExternalUrl"];
        if (!string.IsNullOrWhiteSpace(externalUrl))
        {
            // site externo fora do ar só deixa o relatório Degraded, não derruba a API
            builder.AddUrlGroup(new Uri(externalUrl), name: "fiap-site",
                failureStatus: HealthStatus.Degraded, tags: ["external"], timeout: TimeSpan.FromSeconds(5));
        }

        return services;
    }

    public static IEndpointConventionBuilder MapRecommendaHealthChecks(this IEndpointRouteBuilder app)
    {
        return app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        })
        .AllowAnonymous()
        .DisableRateLimiting();
    }
}
