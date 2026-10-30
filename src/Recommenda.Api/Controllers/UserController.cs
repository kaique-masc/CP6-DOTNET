using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recommenda.Api.Security;
using Recommenda.Application.DTOs.Users;
using Recommenda.Application.Services;

namespace Recommenda.Api.Controllers;

/// <summary>
/// Cadastro e manutenção de usuários.
/// </summary>
[ApiController]
[ApiVersionNeutral]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os usuários.
    /// </summary>
    /// <response code="200">Lista de usuários.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _userService.GetAllAsync(cancellationToken));

    /// <summary>
    /// Busca um usuário pelo id.
    /// </summary>
    /// <param name="id">Id do usuário.</param>
    /// <response code="200">Usuário encontrado.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _userService.GetByIdAsync(id, cancellationToken));

    /// <summary>
    /// Cadastra um novo usuário (endpoint público).
    /// </summary>
    /// <remarks>
    /// A senha precisa ter no mínimo 8 caracteres, com letras e números. O usuário deve ter pelo menos 13 anos.
    /// Todo usuário criado por aqui recebe o papel <c>User</c>.
    ///
    ///     POST /api/user
    ///     {
    ///         "name": "Ana Souza",
    ///         "email": "ana@example.com",
    ///         "password": "senha1234",
    ///         "birthDate": "2000-05-10"
    ///     }
    /// </remarks>
    /// <response code="201">Usuário criado.</response>
    /// <response code="400">Dados inválidos ou regra de negócio violada.</response>
    /// <response code="409">E-mail já cadastrado.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cadastrando usuário {Email}. TraceId: {TraceId}", request.Email, HttpContext.TraceIdentifier);

        var user = await _userService.CreateAsync(request, cancellationToken);

        _logger.LogInformation("Usuário {UserId} cadastrado. TraceId: {TraceId}", user.Id, HttpContext.TraceIdentifier);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Atualiza nome e data de nascimento de um usuário.
    /// </summary>
    /// <response code="204">Usuário atualizado.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Remove um usuário. Exige papel <c>Admin</c>.
    /// </summary>
    /// <response code="204">Usuário removido.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="403">Token sem o papel Admin.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Retorna as configurações de recomendação do usuário.
    /// </summary>
    /// <response code="200">Configuração encontrada.</response>
    /// <response code="404">Usuário ou configuração não encontrados.</response>
    [HttpGet("{id:guid}/configuration")]
    [ProducesResponseType(typeof(UserConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConfiguration(Guid id, CancellationToken cancellationToken)
        => Ok(await _userService.GetConfigurationAsync(id, cancellationToken));

    /// <summary>
    /// Cria ou atualiza as configurações de recomendação do usuário.
    /// </summary>
    /// <remarks>O relacionamento é 1:0..1 — cada usuário tem no máximo uma configuração.</remarks>
    /// <response code="200">Configuração salva.</response>
    /// <response code="400">Valores fora das regras (ex.: maxRecommendations fora de 1–50).</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpPut("{id:guid}/configuration")]
    [Authorize]
    [ProducesResponseType(typeof(UserConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SaveConfiguration(Guid id, [FromBody] UserConfigurationRequest request, CancellationToken cancellationToken)
        => Ok(await _userService.SaveConfigurationAsync(id, request, cancellationToken));
}
