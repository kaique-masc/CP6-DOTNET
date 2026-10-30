using Microsoft.EntityFrameworkCore;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Domain.Entities;
using Recommenda.Infrastructure.Persistence;

namespace Recommenda.Infrastructure.Repositories;

public class RatingRepository : Repository<Rating>, IRatingRepository
{
    public RatingRepository(RecommendaContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Rating>> GetByContentAsync(Guid contentId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(r => r.ContentId == contentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsForUserAndContentAsync(Guid userId, Guid contentId, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(r => r.UserId == userId && r.ContentId == contentId, cancellationToken);
}
