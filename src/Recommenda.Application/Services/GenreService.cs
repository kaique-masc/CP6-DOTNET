using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.DTOs.Genres;
using Recommenda.Application.Mappings;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Services;

public class GenreService : IGenreService
{
    private readonly IRepository<Genre> _genres;
    private readonly IUnitOfWork _unitOfWork;

    public GenreService(IRepository<Genre> genres, IUnitOfWork unitOfWork)
    {
        _genres = genres;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<GenreResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var genres = await _genres.GetAllAsync(cancellationToken);
        return genres.OrderBy(g => g.Name).Select(g => g.ToResponse()).ToList();
    }

    public async Task<GenreResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var genre = await _genres.GetByIdAsync(id, cancellationToken)
                    ?? throw new ResourceNotFoundException("Gênero", id);

        return genre.ToResponse();
    }

    public async Task<GenreResponse> CreateAsync(GenreRequest request, CancellationToken cancellationToken = default)
    {
        var genre = new Genre(request.Name, request.Description);

        if (await _genres.AnyAsync(g => g.Name == genre.Name, cancellationToken))
            throw new ConflictException($"Já existe um gênero com o nome '{genre.Name}'.");

        await _genres.AddAsync(genre, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return genre.ToResponse();
    }

    public async Task UpdateAsync(Guid id, GenreRequest request, CancellationToken cancellationToken = default)
    {
        var genre = await _genres.GetByIdAsync(id, cancellationToken)
                    ?? throw new ResourceNotFoundException("Gênero", id);

        var name = request.Name.Trim();
        if (await _genres.AnyAsync(g => g.Name == name && g.Id != id, cancellationToken))
            throw new ConflictException($"Já existe um gênero com o nome '{name}'.");

        genre.Update(request.Name, request.Description);
        _genres.Update(genre);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var genre = await _genres.GetByIdAsync(id, cancellationToken)
                    ?? throw new ResourceNotFoundException("Gênero", id);

        _genres.Delete(genre);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
