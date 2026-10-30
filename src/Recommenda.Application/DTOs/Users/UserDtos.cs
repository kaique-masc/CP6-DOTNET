using System.ComponentModel.DataAnnotations;

namespace Recommenda.Application.DTOs.Users;

public sealed record CreateUserRequest
{
    /// <example>Ana Souza</example>
    [Required, StringLength(120)]
    public string Name { get; init; } = string.Empty;

    /// <example>ana@example.com</example>
    [Required, EmailAddress, StringLength(180)]
    public string Email { get; init; } = string.Empty;

    /// <example>senha1234</example>
    [Required, MinLength(8)]
    public string Password { get; init; } = string.Empty;

    /// <example>2000-05-10</example>
    [Required]
    public DateOnly BirthDate { get; init; }
}

public sealed record UpdateUserRequest
{
    [Required, StringLength(120)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public DateOnly BirthDate { get; init; }
}

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    DateOnly BirthDate,
    int Age,
    string Role,
    DateTime CreatedAt);

public sealed record UserConfigurationRequest
{
    /// <example>pt-BR</example>
    [Required, StringLength(10)]
    public string PreferredLanguage { get; init; } = "pt-BR";

    public bool AllowAdultContent { get; init; }

    public bool ReceiveNotifications { get; init; } = true;

    /// <example>10</example>
    [Range(1, 50)]
    public int MaxRecommendations { get; init; } = 10;
}

public sealed record UserConfigurationResponse(
    Guid Id,
    Guid UserId,
    string PreferredLanguage,
    bool AllowAdultContent,
    bool ReceiveNotifications,
    int MaxRecommendations,
    DateTime? UpdatedAt);
