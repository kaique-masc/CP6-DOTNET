using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.DTOs.Ratings;
using Recommenda.Application.Mappings;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratings;
    private readonly IRepository<User> _users;
    private readonly IRepository<Content> _contents;
    private readonly IUnitOfWork _unitOfWork;

    public RatingService(IRatingRepository ratings, IRepository<User> users, IRepository<Content> contents, IUnitOfWork unitOfWork)
    {
        _ratings = ratings;
        _users = users;
        _contents = contents;
        _unitOfWork = unitOfWork;
    }

    public async Task<RatingResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rating = await _ratings.GetByIdAsync(id, cancellationToken)
                     ?? throw new ResourceNotFoundException("Avaliação", id);

        return rating.ToResponse();
    }

    public async Task<IReadOnlyList<RatingResponse>> GetByContentAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        if (!await _contents.ExistsByIdAsync(contentId, cancellationToken))
            throw new ResourceNotFoundException("Conteúdo", contentId);

        var ratings = await _ratings.GetByContentAsync(contentId, cancellationToken);
        return ratings.Select(r => r.ToResponse()).ToList();
    }

    public async Task<RatingResponse> CreateAsync(CreateRatingRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _users.ExistsByIdAsync(request.UserId, cancellationToken))
            throw new ResourceNotFoundException("Usuário", request.UserId);

        if (!await _contents.ExistsByIdAsync(request.ContentId, cancellationToken))
            throw new ResourceNotFoundException("Conteúdo", request.ContentId);

        if (await _ratings.ExistsForUserAndContentAsync(request.UserId, request.ContentId, cancellationToken))
            throw new ConflictException("Este usuário já avaliou este conteúdo.");

        var rating = new Rating(request.UserId, request.ContentId, request.Score, request.Comment);

        await _ratings.AddAsync(rating, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rating.ToResponse();
    }

    public async Task UpdateAsync(Guid id, UpdateRatingRequest request, CancellationToken cancellationToken = default)
    {
        var rating = await _ratings.GetByIdAsync(id, cancellationToken)
                     ?? throw new ResourceNotFoundException("Avaliação", id);

        rating.Update(request.Score, request.Comment);
        _ratings.Update(rating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rating = await _ratings.GetByIdAsync(id, cancellationToken)
                     ?? throw new ResourceNotFoundException("Avaliação", id);

        _ratings.Delete(rating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
