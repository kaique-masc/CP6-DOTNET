using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Recommenda.Api.HealthChecks;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        var includeErrors = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment();

        var payload = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            timestamp = DateTime.UtcNow,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.ToString(),
                description = entry.Value.Description,
                tags = entry.Value.Tags,
                error = includeErrors ? entry.Value.Exception?.Message : null
            })
        };

        context.Response.ContentType = "application/json; charset=utf-8";
        return context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }
}
