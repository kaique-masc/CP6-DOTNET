using System.ComponentModel.DataAnnotations;
using Recommenda.Domain.Enums;

namespace Recommenda.Application.DTOs.Contents;

public sealed record ContentRequest
{
    /// <example>A Bruxa</example>
    [Required, StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [StringLength(2000)]
    public string? Synopsis { get; init; }

    /// <example>Movie</example>
    [Required]
    public ContentType Type { get; init; }

    /// <example>2015</example>
    public int ReleaseYear { get; init; }

    /// <example>92</example>
    [Range(1, 100000)]
    public int DurationMinutes { get; init; }

    /// <example>16</example>
    public int AgeRating { get; init; }

    [Required, MinLength(1)]
    public List<Guid> GenreIds { get; init; } = new();
}

public sealed record ContentResponse(
    Guid Id,
    string Title,
    string? Synopsis,
    string Type,
    int ReleaseYear,
    int DurationMinutes,
    int AgeRating,
    DateTime CreatedAt);

public sealed record GenreSummary(Guid Id, string Name);

public sealed record ContentDetailResponse(
    Guid Id,
    string Title,
    string? Synopsis,
    string Type,
    int ReleaseYear,
    int DurationMinutes,
    int AgeRating,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<GenreSummary> Genres);
