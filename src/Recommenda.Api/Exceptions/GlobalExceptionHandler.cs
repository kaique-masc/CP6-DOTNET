using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Recommenda.Application.Exceptions;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;
        var (status, title) = Map(exception);

        _logger.LogError(exception,
            "Erro ao processar {Method} {Path}. Status: {StatusCode}. TraceId: {TraceId}",
            httpContext.Request.Method, httpContext.Request.Path, status, traceId);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Type = $"https://httpstatuses.io/{status}",
            Instance = httpContext.Request.Path,
            Detail = status == StatusCodes.Status500InternalServerError && !_environment.IsDevelopment()
                ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                : exception.Message
        };

        if (_environment.IsDevelopment())
            problem.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json", cancellationToken);

        return true;
    }

    private static (int Status, string Title) Map(Exception exception) => exception switch
    {
        ValidationException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
        DomainException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
        ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
        BadHttpRequestException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
        ResourceNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
        KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
        ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
        _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
    };
}
