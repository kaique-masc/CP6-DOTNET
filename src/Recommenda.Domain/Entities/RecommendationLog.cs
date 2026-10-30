namespace Recommenda.Domain.Entities;

// Documento guardado no MongoDB (coleção recommendationLogs), fora do contexto relacional.
public class RecommendationLog
{
    public RecommendationLog(Guid id, Guid userId, string query, IEnumerable<string> titles, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Query = query;
        Titles = titles.ToList();
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public string Query { get; private set; }
    public IReadOnlyList<string> Titles { get; private set; }
    public DateTime CreatedAt { get; }

    public static RecommendationLog Create(Guid userId, string query, IEnumerable<string> titles)
        => new(Guid.NewGuid(), userId, query.Trim(), titles.Select(t => t.Trim()), DateTime.UtcNow);

    public void Update(string query, IEnumerable<string> titles)
    {
        Query = query.Trim();
        Titles = titles.Select(t => t.Trim()).ToList();
    }
}
