using Recommenda.Application.DTOs.Ratings;

namespace Recommenda.Application.Services;

public interface IRatingService
{
    Task<RatingResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RatingResponse>> GetByContentAsync(Guid contentId, CancellationToken cancellationToken = default);
    Task<RatingResponse> CreateAsync(CreateRatingRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateRatingRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
