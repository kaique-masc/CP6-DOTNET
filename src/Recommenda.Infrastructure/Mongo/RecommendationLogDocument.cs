using MongoDB.Bson.Serialization.Attributes;

namespace Recommenda.Infrastructure.Mongo;

public class RecommendationLogDocument
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("userId")]
    public Guid UserId { get; set; }

    [BsonElement("query")]
    public string Query { get; set; } = string.Empty;

    [BsonElement("titles")]
    public List<string> Titles { get; set; } = new();

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }
}
