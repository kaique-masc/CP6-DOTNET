namespace Recommenda.Application.DTOs.RecommendationLogs;

public sealed record CreateRecommendationLogRequest
{
    /// <example>11111111-1111-1111-1111-111111111111</example>
    public Guid UserId { get; init; }

    /// <example>filme de terror curto</example>
    public string? Query { get; init; }

    public List<string>? Titles { get; init; }
}

public sealed record UpdateRecommendationLogRequest
{
    /// <example>filme de terror curto, dublado</example>
    public string? Query { get; init; }

    public List<string>? Titles { get; init; }
}

public sealed record RecommendationLogItem(
    Guid Id,
    Guid UserId,
    string Query,
    IReadOnlyList<string> Titles,
    DateTime CreatedAt);
