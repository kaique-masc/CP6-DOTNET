using Recommenda.Domain.Entities;

namespace Recommenda.Application.Abstractions.Persistence;

public interface IContentRepository : IRepository<Content>
{
    Task<Content?> GetWithGenresAsync(Guid id, CancellationToken cancellationToken = default);
}
