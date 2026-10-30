using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recommenda.Api.Security;
using Recommenda.Application.DTOs.Genres;
using Recommenda.Application.Services;

namespace Recommenda.Api.Controllers;

/// <summary>
/// Gêneros usados para classificar os conteúdos. Escrita restrita ao papel Admin.
/// </summary>
[ApiController]
[ApiVersionNeutral]
[Route("api/genre")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    /// <summary>
    /// Lista os gêneros em ordem alfabética.
    /// </summary>
    /// <response code="200">Lista de gêneros.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GenreResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _genreService.GetAllAsync(cancellationToken));

    /// <summary>
    /// Busca um gênero pelo id.
    /// </summary>
    /// <response code="200">Gênero encontrado.</response>
    /// <response code="404">Gênero não encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _genreService.GetByIdAsync(id, cancellationToken));

    /// <summary>
    /// Cadastra um gênero. Exige papel <c>Admin</c>.
    /// </summary>
    /// <response code="201">Gênero criado.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="403">Token sem o papel Admin.</response>
    /// <response code="409">Já existe um gênero com esse nome.</response>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] GenreRequest request, CancellationToken cancellationToken)
    {
        var genre = await _genreService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genre);
    }

    /// <summary>
    /// Atualiza um gênero. Exige papel <c>Admin</c>.
    /// </summary>
    /// <response code="204">Gênero atualizado.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="403">Token sem o papel Admin.</response>
    /// <response code="404">Gênero não encontrado.</response>
    /// <response code="409">Já existe um gênero com esse nome.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] GenreRequest request, CancellationToken cancellationToken)
    {
        await _genreService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Remove um gênero. Exige papel <c>Admin</c>.
    /// </summary>
    /// <remarks>Gêneros vinculados a algum conteúdo não podem ser removidos (409).</remarks>
    /// <response code="204">Gênero removido.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="403">Token sem o papel Admin.</response>
    /// <response code="404">Gênero não encontrado.</response>
    /// <response code="409">Gênero em uso por algum conteúdo.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _genreService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
