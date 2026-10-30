using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recommenda.Application.DTOs.Auth;
using Recommenda.Application.Services;

namespace Recommenda.Api.Controllers;

/// <summary>
/// Autenticação via JWT.
/// </summary>
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Autentica o usuário por e-mail e senha e devolve um JWT.
    /// </summary>
    /// <remarks>
    ///     POST /api/v2/auth/login
    ///     {
    ///         "email": "ana@example.com",
    ///         "password": "senha1234"
    ///     }
    ///
    /// Use o <c>accessToken</c> retornado no header <c>Authorization: Bearer {token}</c>.
    /// </remarks>
    /// <response code="200">Login realizado. Retorna o token e os dados do usuário.</response>
    /// <response code="400">Corpo da requisição inválido.</response>
    /// <response code="401">E-mail ou senha inválidos.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (result is null)
        {
            _logger.LogWarning("Tentativa de login recusada para {Email}. TraceId: {TraceId}", request.Email, HttpContext.TraceIdentifier);
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Credenciais inválidas",
                detail: "E-mail ou senha inválidos.");
        }

        _logger.LogInformation("Login realizado para {Email} com papel {Role}. TraceId: {TraceId}",
            result.Email, result.Role, HttpContext.TraceIdentifier);
        return Ok(result);
    }

    /// <summary>
    /// Retorna as claims do token enviado.
    /// </summary>
    /// <response code="200">Lista de claims do token.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new ClaimItem(c.Type, c.Value)).ToList();
        return Ok(new CurrentUserResponse(claims));
    }
}
