using Recommenda.Application.DTOs.Auth;

namespace Recommenda.Application.Services;

public interface IAuthService
{
    /// <summary>Retorna null quando o e-mail não existe ou a senha não confere.</summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
