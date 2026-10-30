using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.DTOs.RecommendationLogs;
using Recommenda.Application.Exceptions;
using Recommenda.Application.Mappings;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Services;

public class RecommendationLogService : IRecommendationLogService
{
    public const int ListLimit = 20;

    private readonly IRecommendationLogRepository _logs;

    public RecommendationLogService(IRecommendationLogRepository logs)
    {
        _logs = logs;
    }

    public async Task<RecommendationLogItem> CreateAsync(CreateRecommendationLogRequest request, CancellationToken cancellationToken = default)
    {
        ValidateUserId(request.UserId);
        Validate(request.Query, request.Titles);

        var log = RecommendationLog.Create(request.UserId, request.Query!, request.Titles!);
        await _logs.InsertAsync(log, cancellationToken);

        return log.ToItem();
    }

    public async Task<RecommendationLogItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var log = await _logs.GetByIdAsync(id, cancellationToken)
                  ?? throw new ResourceNotFoundException("Histórico de recomendação", id);

        return log.ToItem();
    }

    public async Task<IReadOnlyList<RecommendationLogItem>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);

        var logs = await _logs.GetByUserAsync(userId, ListLimit, cancellationToken);
        return logs.Select(l => l.ToItem()).ToList();
    }

    public async Task UpdateAsync(Guid id, UpdateRecommendationLogRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Query, request.Titles);

        var titles = request.Titles!.Select(t => t.Trim()).ToList();
        var updated = await _logs.UpdateAsync(id, request.Query!.Trim(), titles, cancellationToken);

        if (!updated)
            throw new ResourceNotFoundException("Histórico de recomendação", id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await _logs.DeleteAsync(id, cancellationToken))
            throw new ResourceNotFoundException("Histórico de recomendação", id);
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("O campo 'userId' é obrigatório e não pode ser um Guid vazio.");
    }

    private static void Validate(string? query, IReadOnlyCollection<string>? titles)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ValidationException("O campo 'query' é obrigatório.");

        if (titles is null || titles.Count == 0)
            throw new ValidationException("O campo 'titles' deve conter pelo menos um título.");

        if (titles.Any(string.IsNullOrWhiteSpace))
            throw new ValidationException("O campo 'titles' não pode conter itens em branco.");
    }
}
