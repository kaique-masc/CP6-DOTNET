using Microsoft.EntityFrameworkCore;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Domain.Entities;
using Recommenda.Infrastructure.Persistence;

namespace Recommenda.Infrastructure.Repositories;

public class ContentRepository : Repository<Content>, IContentRepository
{
    public ContentRepository(RecommendaContext context) : base(context)
    {
    }

    public Task<Content?> GetWithGenresAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet
            .Include(c => c.ContentGenres)
            .ThenInclude(cg => cg.Genre)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
