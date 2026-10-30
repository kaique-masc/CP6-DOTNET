using Recommenda.Domain.Common;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Domain.Entities;

public class Rating : BaseEntity
{
    public const int MinScore = 1;
    public const int MaxScore = 5;
    public const int CommentMaxLength = 500;

    protected Rating()
    {
    }

    public Rating(Guid userId, Guid contentId, int score, string? comment = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("A avaliação precisa de um usuário.");

        if (contentId == Guid.Empty)
            throw new DomainException("A avaliação precisa de um conteúdo.");

        UserId = userId;
        ContentId = contentId;
        SetValues(score, comment);
    }

    public Guid UserId { get; private set; }
    public Guid ContentId { get; private set; }
    public int Score { get; private set; }
    public string? Comment { get; private set; }

    public User? User { get; private set; }
    public Content? Content { get; private set; }

    public void Update(int score, string? comment)
    {
        SetValues(score, comment);
        Touch();
    }

    private void SetValues(int score, string? comment)
    {
        if (score < MinScore || score > MaxScore)
            throw new DomainException($"A nota deve estar entre {MinScore} e {MaxScore}.");

        if (comment?.Length > CommentMaxLength)
            throw new DomainException($"O comentário deve ter no máximo {CommentMaxLength} caracteres.");

        Score = score;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }
}
