using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Recommenda.Api.Extensions;

public static class RateLimitingExtensions
{
    public const string WritePolicy = "fixed-write";
    public const string ReadPolicy = "fixed-read";

    public static IServiceCollection AddRecommendaRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var writeLimit = configuration.GetValue("RateLimiting:Write:PermitLimit", 10);
        var writeWindow = configuration.GetValue("RateLimiting:Write:WindowSeconds", 60);
        var readLimit = configuration.GetValue("RateLimiting:Read:PermitLimit", 60);
        var readWindow = configuration.GetValue("RateLimiting:Read:WindowSeconds", 60);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(WritePolicy, context => FixedWindowByIp(context, WritePolicy, writeLimit, writeWindow));
            options.AddPolicy(ReadPolicy, context => FixedWindowByIp(context, ReadPolicy, readLimit, readWindow));

            options.OnRejected = async (context, cancellationToken) =>
            {
                var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var value)
                    ? (int)Math.Ceiling(value.TotalSeconds)
                    : writeWindow;

                var http = context.HttpContext;
                http.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                http.Response.Headers.RetryAfter = retryAfter.ToString(CultureInfo.InvariantCulture);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Muitas requisições",
                    Detail = $"Limite de requisições atingido. Tente novamente em {retryAfter} segundos.",
                    Instance = http.Request.Path
                };
                problem.Extensions["retryAfterSeconds"] = retryAfter;
                problem.Extensions["traceId"] = http.TraceIdentifier;

                await http.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json", cancellationToken);
            };
        });

        return services;
    }

    private static RateLimitPartition<string> FixedWindowByIp(HttpContext context, string policy, int permitLimit, int windowSeconds)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter($"{policy}:{ip}", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = TimeSpan.FromSeconds(windowSeconds),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    }
}
