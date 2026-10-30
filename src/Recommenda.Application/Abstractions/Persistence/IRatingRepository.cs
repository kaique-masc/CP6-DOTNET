using Recommenda.Domain.Entities;

namespace Recommenda.Application.Abstractions.Persistence;

public interface IRatingRepository : IRepository<Rating>
{
    Task<IReadOnlyList<Rating>> GetByContentAsync(Guid contentId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAndContentAsync(Guid userId, Guid contentId, CancellationToken cancellationToken = default);
}
