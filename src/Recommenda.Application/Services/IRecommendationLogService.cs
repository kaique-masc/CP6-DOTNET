using Recommenda.Application.DTOs.RecommendationLogs;

namespace Recommenda.Application.Services;

public interface IRecommendationLogService
{
    Task<RecommendationLogItem> CreateAsync(CreateRecommendationLogRequest request, CancellationToken cancellationToken = default);
    Task<RecommendationLogItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RecommendationLogItem>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateRecommendationLogRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
