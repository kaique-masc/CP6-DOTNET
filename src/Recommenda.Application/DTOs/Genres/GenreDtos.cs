using System.ComponentModel.DataAnnotations;

namespace Recommenda.Application.DTOs.Genres;

public sealed record GenreRequest
{
    /// <example>Terror</example>
    [Required, StringLength(60)]
    public string Name { get; init; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; init; }
}

public sealed record GenreResponse(Guid Id, string Name, string? Description, DateTime CreatedAt);
