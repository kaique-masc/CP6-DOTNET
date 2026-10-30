using Recommenda.Application.DTOs.Genres;

namespace Recommenda.Application.Services;

public interface IGenreService
{
    Task<IReadOnlyList<GenreResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GenreResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GenreResponse> CreateAsync(GenreRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, GenreRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
