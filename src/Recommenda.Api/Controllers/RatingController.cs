using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recommenda.Application.DTOs.Ratings;
using Recommenda.Application.Services;

namespace Recommenda.Api.Controllers;

/// <summary>
/// Avaliações (nota de 1 a 5) que os usuários dão aos conteúdos.
/// </summary>
[ApiController]
[ApiVersionNeutral]
[Route("api/rating")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    /// <summary>
    /// Busca uma avaliação pelo id.
    /// </summary>
    /// <response code="200">Avaliação encontrada.</response>
    /// <response code="404">Avaliação não encontrada.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _ratingService.GetByIdAsync(id, cancellationToken));

    /// <summary>
    /// Lista as avaliações de um conteúdo, da mais recente para a mais antiga.
    /// </summary>
    /// <response code="200">Avaliações do conteúdo.</response>
    /// <response code="404">Conteúdo não encontrado.</response>
    [HttpGet("content/{contentId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<RatingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByContent(Guid contentId, CancellationToken cancellationToken)
        => Ok(await _ratingService.GetByContentAsync(contentId, cancellationToken));

    /// <summary>
    /// Registra a avaliação de um usuário para um conteúdo.
    /// </summary>
    /// <remarks>Cada usuário pode avaliar o mesmo conteúdo apenas uma vez.</remarks>
    /// <response code="201">Avaliação criada.</response>
    /// <response code="400">Nota fora do intervalo 1–5 ou dados inválidos.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Usuário ou conteúdo não encontrado.</response>
    /// <response code="409">O usuário já avaliou esse conteúdo.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateRatingRequest request, CancellationToken cancellationToken)
    {
        var rating = await _ratingService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
    }

    /// <summary>
    /// Altera a nota ou o comentário de uma avaliação.
    /// </summary>
    /// <response code="204">Avaliação atualizada.</response>
    /// <response code="400">Nota fora do intervalo 1–5.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Avaliação não encontrada.</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRatingRequest request, CancellationToken cancellationToken)
    {
        await _ratingService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Remove uma avaliação.
    /// </summary>
    /// <response code="204">Avaliação removida.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Avaliação não encontrada.</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _ratingService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
