using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Recommenda.Api.Extensions;
using Recommenda.Api.Security;
using Recommenda.Application.Common;
using Recommenda.Application.DTOs.Contents;
using Recommenda.Application.Services;

namespace Recommenda.Api.Controllers;

/// <summary>
/// Catálogo de conteúdos (filmes, séries, documentários e animes).
/// </summary>
/// <remarks>
/// Recurso versionado: a v1 (deprecada) devolve a lista completa; a v2 devolve a listagem paginada.
/// </remarks>
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/content")]
[Route("api/v{version:apiVersion}/content")]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;
    private readonly ILogger<ContentController> _logger;

    public ContentController(IContentService contentService, ILogger<ContentController> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    /// <summary>
    /// [v1 - deprecada] Lista todos os conteúdos.
    /// </summary>
    /// <remarks>Contrato antigo: devolve um array com todos os registros, sem paginação.</remarks>
    /// <response code="200">Array com todos os conteúdos.</response>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IReadOnlyList<ContentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _contentService.GetAllAsync(cancellationToken));

    /// <summary>
    /// [v2] Lista os conteúdos de forma paginada.
    /// </summary>
    /// <remarks>
    /// Ordenação estável por data de criação. Página além do total devolve <c>items</c> vazio.
    ///
    ///     GET /api/content?page=1&amp;pageSize=5
    /// </remarks>
    /// <param name="page">Número da página (mínimo 1). Padrão: 1.</param>
    /// <param name="pageSize">Itens por página (1 a 100). Padrão: 20.</param>
    /// <response code="200">Envelope com a página solicitada e os totais.</response>
    /// <response code="400">page ou pageSize fora do intervalo permitido.</response>
    /// <response code="429">Limite de requisições da listagem atingido.</response>
    [HttpGet]
    [MapToApiVersion("2.0")]
    [EnableRateLimiting(RateLimitingExtensions.ReadPolicy)]
    [ProducesResponseType(typeof(PagedResult<ContentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = PageRequest.DefaultPage,
        [FromQuery] int pageSize = PageRequest.DefaultPageSize,
        CancellationToken cancellationToken = default)
        => Ok(await _contentService.GetPagedAsync(page, pageSize, cancellationToken));

    /// <summary>
    /// Busca um conteúdo pelo id, com seus gêneros.
    /// </summary>
    /// <param name="id">Id do conteúdo.</param>
    /// <response code="200">Conteúdo encontrado.</response>
    /// <response code="404">Conteúdo não encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContentDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _contentService.GetByIdAsync(id, cancellationToken));

    /// <summary>
    /// Cadastra um novo conteúdo.
    /// </summary>
    /// <remarks>
    /// Exige token JWT. Limite de 10 requisições por minuto por IP.
    ///
    ///     POST /api/content
    ///     {
    ///         "title": "O Iluminado",
    ///         "synopsis": "Um escritor aceita cuidar de um hotel isolado durante o inverno.",
    ///         "type": "Movie",
    ///         "releaseYear": 1980,
    ///         "durationMinutes": 146,
    ///         "ageRating": 16,
    ///         "genreIds": [ "15b32338-5b8c-42e5-88ad-bb279b43582d" ]
    ///     }
    /// </remarks>
    /// <response code="201">Conteúdo criado.</response>
    /// <response code="400">Dados inválidos ou regra de negócio violada.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Algum gênero informado não existe.</response>
    /// <response code="429">Limite de requisições atingido.</response>
    [HttpPost]
    [Authorize]
    [EnableRateLimiting(RateLimitingExtensions.WritePolicy)]
    [ProducesResponseType(typeof(ContentDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Create([FromBody] ContentRequest request, ApiVersion apiVersion, CancellationToken cancellationToken)
    {
        var traceId = HttpContext.TraceIdentifier;
        _logger.LogInformation("Iniciando cadastro de conteúdo {Title} ({Type}) com {GenreCount} gênero(s). TraceId: {TraceId}",
            request.Title, request.Type, request.GenreIds.Count, traceId);

        var content = await _contentService.CreateAsync(request, cancellationToken);

        _logger.LogInformation("Conteúdo {ContentId} cadastrado com sucesso. TraceId: {TraceId}", content.Id, traceId);
        return CreatedAtAction(nameof(GetById), new { id = content.Id, version = apiVersion.ToString() }, content);
    }

    /// <summary>
    /// Atualiza um conteúdo existente.
    /// </summary>
    /// <response code="204">Conteúdo atualizado.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Conteúdo ou gênero não encontrado.</response>
    /// <response code="429">Limite de requisições atingido.</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [EnableRateLimiting(RateLimitingExtensions.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContentRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando conteúdo {ContentId}. TraceId: {TraceId}", id, HttpContext.TraceIdentifier);

        await _contentService.UpdateAsync(id, request, cancellationToken);

        _logger.LogInformation("Conteúdo {ContentId} atualizado. TraceId: {TraceId}", id, HttpContext.TraceIdentifier);
        return NoContent();
    }

    /// <summary>
    /// Remove um conteúdo. Exige papel <c>Admin</c>.
    /// </summary>
    /// <response code="204">Conteúdo removido.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="403">Token sem o papel Admin.</response>
    /// <response code="404">Conteúdo não encontrado.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _contentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
