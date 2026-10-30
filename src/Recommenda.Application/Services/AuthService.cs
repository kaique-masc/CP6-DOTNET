using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.Abstractions.Security;
using Recommenda.Application.DTOs.Auth;

namespace Recommenda.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository users, ITokenService tokenService)
    {
        _users = users;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrEmpty(request.Password))
            return null;

        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !user.VerifyPassword(request.Password))
            return null;

        var token = _tokenService.Generate(user);
        return new LoginResponse(token.Token, token.ExpiresAt, user.Email, user.Name, user.Role.ToString());
    }
}
