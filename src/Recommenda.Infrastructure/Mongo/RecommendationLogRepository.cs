using MongoDB.Driver;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Domain.Entities;

namespace Recommenda.Infrastructure.Mongo;

public class RecommendationLogRepository : IRecommendationLogRepository
{
    private readonly IMongoCollection<RecommendationLogDocument> _collection;

    public RecommendationLogRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<RecommendationLogDocument>(MongoCollections.RecommendationLogs);
    }

    public Task InsertAsync(RecommendationLog log, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(ToDocument(log), cancellationToken: cancellationToken);

    public async Task<RecommendationLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var document = await _collection.Find(d => d.Id == id).FirstOrDefaultAsync(cancellationToken);
        return document is null ? null : ToEntity(document);
    }

    public async Task<IReadOnlyList<RecommendationLog>> GetByUserAsync(Guid userId, int limit, CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find(d => d.UserId == userId)
            .SortByDescending(d => d.CreatedAt)
            .Limit(limit)
            .ToListAsync(cancellationToken);

        return documents.Select(ToEntity).ToList();
    }

    public async Task<bool> UpdateAsync(Guid id, string query, IReadOnlyList<string> titles, CancellationToken cancellationToken = default)
    {
        var update = Builders<RecommendationLogDocument>.Update
            .Set(d => d.Query, query)
            .Set(d => d.Titles, titles.ToList());

        var result = await _collection.UpdateOneAsync(d => d.Id == id, update, cancellationToken: cancellationToken);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(d => d.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

    private static RecommendationLogDocument ToDocument(RecommendationLog log) => new()
    {
        Id = log.Id,
        UserId = log.UserId,
        Query = log.Query,
        Titles = log.Titles.ToList(),
        CreatedAt = log.CreatedAt
    };

    private static RecommendationLog ToEntity(RecommendationLogDocument document)
        => new(document.Id, document.UserId, document.Query, document.Titles, document.CreatedAt);
}
