using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Recommenda.Infrastructure.Mongo;

// Cria o índice (userId, createdAt desc) usado pela listagem por usuário.
public class RecommendationLogIndexInitializer : IHostedService
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<RecommendationLogIndexInitializer> _logger;

    public RecommendationLogIndexInitializer(IMongoDatabase database, ILogger<RecommendationLogIndexInitializer> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var collection = _database.GetCollection<RecommendationLogDocument>(MongoCollections.RecommendationLogs);
            var keys = Builders<RecommendationLogDocument>.IndexKeys
                .Ascending(d => d.UserId)
                .Descending(d => d.CreatedAt);

            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<RecommendationLogDocument>(keys, new CreateIndexOptions { Name = "ix_userId_createdAt" }),
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível criar o índice da coleção {Collection}", MongoCollections.RecommendationLogs);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
