using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.Common;
using Recommenda.Application.DTOs.Contents;
using Recommenda.Application.Mappings;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Services;

public class ContentService : IContentService
{
    private readonly IContentRepository _contents;
    private readonly IRepository<Genre> _genres;
    private readonly IUnitOfWork _unitOfWork;

    public ContentService(IContentRepository contents, IRepository<Genre> genres, IUnitOfWork unitOfWork)
    {
        _contents = contents;
        _genres = genres;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ContentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var contents = await _contents.GetAllAsync(cancellationToken);
        return contents.Select(c => c.ToResponse()).ToList();
    }

    public async Task<PagedResult<ContentResponse>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var request = PageRequest.Create(page, pageSize);
        var (items, total) = await _contents.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

        return PagedResult<ContentResponse>.Create(items.Select(c => c.ToResponse()).ToList(), request, total);
    }

    public async Task<ContentDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var content = await _contents.GetWithGenresAsync(id, cancellationToken)
                      ?? throw new ResourceNotFoundException("Conteúdo", id);

        return content.ToDetailResponse();
    }

    public async Task<ContentDetailResponse> CreateAsync(ContentRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureGenresExistAsync(request.GenreIds, cancellationToken);

        var content = new Content(request.Title, request.Synopsis, request.Type, request.ReleaseYear,
            request.DurationMinutes, request.AgeRating);
        content.SetGenres(request.GenreIds);

        await _contents.AddAsync(content, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(content.Id, cancellationToken);
    }

    public async Task UpdateAsync(Guid id, ContentRequest request, CancellationToken cancellationToken = default)
    {
        var content = await _contents.GetWithGenresAsync(id, cancellationToken)
                      ?? throw new ResourceNotFoundException("Conteúdo", id);

        await EnsureGenresExistAsync(request.GenreIds, cancellationToken);

        content.Update(request.Title, request.Synopsis, request.Type, request.ReleaseYear,
            request.DurationMinutes, request.AgeRating);
        content.SetGenres(request.GenreIds);

        _contents.Update(content);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var content = await _contents.GetByIdAsync(id, cancellationToken)
                      ?? throw new ResourceNotFoundException("Conteúdo", id);

        _contents.Delete(content);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureGenresExistAsync(IReadOnlyCollection<Guid> genreIds, CancellationToken cancellationToken)
    {
        var ids = genreIds.Distinct().ToList();
        if (ids.Count == 0)
            throw new DomainException("O conteúdo precisa ter pelo menos um gênero.");

        var found = await _genres.FindAsync(g => ids.Contains(g.Id), cancellationToken);
        var missing = ids.FirstOrDefault(id => found.All(g => g.Id != id));

        if (found.Count != ids.Count)
            throw new ResourceNotFoundException("Gênero", missing);
    }
}
