using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Infrastructure.Mongo;
using Recommenda.Infrastructure.Persistence;
using Recommenda.Infrastructure.Repositories;

namespace Recommenda.Infrastructure;

public static class DependencyInjection
{
    public const string MySqlConnectionName = "RecommendaMySql";
    public const string MongoConnectionName = "RecommendaMongo";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMySqlPersistence(configuration);
        services.AddMongoPersistence(configuration);

        return services;
    }

    private static void AddMySqlPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(MySqlConnectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"Connection string '{MySqlConnectionName}' não configurada. Defina ConnectionStrings:{MySqlConnectionName} via user-secrets ou variável de ambiente.");

        services.AddDbContext<RecommendaContext>(options => options.UseMySQL(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IContentRepository, ContentRepository>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddMongoPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(MongoConnectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"Connection string '{MongoConnectionName}' não configurada. Rode: dotnet user-secrets set \"ConnectionStrings:{MongoConnectionName}\" \"mongodb+srv://...\" --project src/Recommenda.Api");

        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(MongoCollections.DatabaseName));

        services.AddScoped<IRecommendationLogRepository, RecommendationLogRepository>();
        services.AddHostedService<RecommendationLogIndexInitializer>();
    }
}
