using Recommenda.Domain.Entities;

namespace Recommenda.Application.Abstractions.Persistence;

public interface IRecommendationLogRepository
{
    Task InsertAsync(RecommendationLog log, CancellationToken cancellationToken = default);

    Task<RecommendationLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecommendationLog>> GetByUserAsync(Guid userId, int limit, CancellationToken cancellationToken = default);

    /// <summary>Altera somente query e titles. Retorna false quando o documento não existe.</summary>
    Task<bool> UpdateAsync(Guid id, string query, IReadOnlyList<string> titles, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
