using System.ComponentModel.DataAnnotations;

namespace Recommenda.Application.DTOs.Auth;

public sealed record LoginRequest
{
    /// <example>ana@example.com</example>
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    /// <example>senha1234</example>
    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed record LoginResponse(
    string AccessToken,
    DateTime Expiration,
    string Email,
    string Name,
    string Role);

public sealed record ClaimItem(string Type, string Value);

public sealed record CurrentUserResponse(IReadOnlyList<ClaimItem> Claims);
