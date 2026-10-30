using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recommenda.Application.DTOs.RecommendationLogs;
using Recommenda.Application.Services;

namespace Recommenda.Api.Controllers;

/// <summary>
/// Histórico de recomendações (armazenado no MongoDB, coleção recommendationLogs).
/// </summary>
[ApiController]
[ApiVersion("2.0")]
[Authorize]
[Route("api/v{version:apiVersion}/recommendation-logs")]
public class RecommendationLogsController : ControllerBase
{
    private readonly IRecommendationLogService _service;
    private readonly ILogger<RecommendationLogsController> _logger;

    public RecommendationLogsController(IRecommendationLogService service, ILogger<RecommendationLogsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Registra o que foi recomendado para um usuário.
    /// </summary>
    /// <remarks>
    ///     POST /api/v2/recommendation-logs
    ///     {
    ///         "userId": "11111111-1111-1111-1111-111111111111",
    ///         "query": "filme de terror curto",
    ///         "titles": [ "A Bruxa", "Hereditário" ]
    ///     }
    /// </remarks>
    /// <response code="201">Histórico criado, com o id gerado.</response>
    /// <response code="400">query vazia, titles vazio/com item em branco ou userId vazio.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RecommendationLogItem), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateRecommendationLogRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Gravando histórico de recomendação para o usuário {UserId}. TraceId: {TraceId}",
            request.UserId, HttpContext.TraceIdentifier);

        var item = await _service.CreateAsync(request, cancellationToken);

        _logger.LogInformation("Histórico {LogId} gravado com {TitleCount} título(s). TraceId: {TraceId}",
            item.Id, item.Titles.Count, HttpContext.TraceIdentifier);
        return CreatedAtAction(nameof(GetById), new { id = item.Id, version = "2.0" }, item);
    }

    /// <summary>
    /// Busca um histórico pelo id.
    /// </summary>
    /// <response code="200">Histórico encontrado.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Histórico não encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecommendationLogItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _service.GetByIdAsync(id, cancellationToken));

    /// <summary>
    /// Lista os históricos de um usuário (mais recentes primeiro, no máximo 20).
    /// </summary>
    /// <response code="200">Array de históricos (pode ser vazio).</response>
    /// <response code="400">userId vazio.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<RecommendationLogItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
        => Ok(await _service.GetByUserAsync(userId, cancellationToken));

    /// <summary>
    /// Atualiza a query e os títulos de um histórico.
    /// </summary>
    /// <remarks>userId, createdAt e id não são alterados.</remarks>
    /// <response code="204">Histórico atualizado.</response>
    /// <response code="400">query vazia ou titles inválido.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Histórico não encontrado.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecommendationLogRequest request, CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Remove um histórico.
    /// </summary>
    /// <response code="204">Histórico removido.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Histórico não encontrado.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
