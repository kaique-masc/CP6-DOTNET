using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Recommenda.Api.Exceptions;
using Recommenda.Api.Extensions;
using Recommenda.Application;
using Recommenda.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = false;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
});

builder.Services
    .AddControllers(options => options.OutputFormatters.RemoveType<StringOutputFormatter>())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRecommendaApiVersioning();
builder.Services.AddRecommendaSwagger(builder.Configuration);
builder.Services.AddRecommendaAuthentication(builder.Configuration);
builder.Services.AddRecommendaRateLimiting(builder.Configuration);
builder.Services.AddRecommendaHealthChecks(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseRecommendaSwagger();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapRecommendaHealthChecks();
app.MapControllers();

app.Run();
