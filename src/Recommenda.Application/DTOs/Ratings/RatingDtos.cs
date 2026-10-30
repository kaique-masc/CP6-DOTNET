using System.ComponentModel.DataAnnotations;

namespace Recommenda.Application.DTOs.Ratings;

public sealed record CreateRatingRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid ContentId { get; init; }

    /// <example>5</example>
    public int Score { get; init; }

    [StringLength(500)]
    public string? Comment { get; init; }
}

public sealed record UpdateRatingRequest
{
    /// <example>4</example>
    public int Score { get; init; }

    [StringLength(500)]
    public string? Comment { get; init; }
}

public sealed record RatingResponse(
    Guid Id,
    Guid UserId,
    Guid ContentId,
    int Score,
    string? Comment,
    DateTime CreatedAt);
