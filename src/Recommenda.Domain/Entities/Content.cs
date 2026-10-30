using Recommenda.Domain.Common;
using Recommenda.Domain.Enums;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Domain.Entities;

public class Content : BaseEntity
{
    public const int TitleMaxLength = 200;
    public const int SynopsisMaxLength = 2000;
    public const int FirstReleaseYear = 1888;

    private readonly List<ContentGenre> _contentGenres = new();
    private readonly List<Rating> _ratings = new();

    protected Content()
    {
    }

    public Content(string title, string? synopsis, ContentType type, int releaseYear, int durationMinutes, int ageRating)
    {
        SetValues(title, synopsis, type, releaseYear, durationMinutes, ageRating);
    }

    public string Title { get; private set; } = string.Empty;
    public string? Synopsis { get; private set; }
    public ContentType Type { get; private set; }
    public int ReleaseYear { get; private set; }
    public int DurationMinutes { get; private set; }
    public int AgeRating { get; private set; }

    public IReadOnlyCollection<ContentGenre> ContentGenres => _contentGenres.AsReadOnly();
    public IReadOnlyCollection<Rating> Ratings => _ratings.AsReadOnly();

    public static readonly int[] AllowedAgeRatings = { 0, 10, 12, 14, 16, 18 };

    public void Update(string title, string? synopsis, ContentType type, int releaseYear, int durationMinutes, int ageRating)
    {
        SetValues(title, synopsis, type, releaseYear, durationMinutes, ageRating);
        Touch();
    }

    public void SetGenres(IEnumerable<Guid> genreIds)
    {
        var ids = genreIds.Where(id => id != Guid.Empty).Distinct().ToList();
        if (ids.Count == 0)
            throw new DomainException("O conteúdo precisa ter pelo menos um gênero.");

        _contentGenres.RemoveAll(cg => !ids.Contains(cg.GenreId));

        foreach (var id in ids.Where(id => _contentGenres.All(cg => cg.GenreId != id)))
            _contentGenres.Add(new ContentGenre(Id, id));
    }

    public bool IsAdultOnly => AgeRating >= 18;

    private void SetValues(string title, string? synopsis, ContentType type, int releaseYear, int durationMinutes, int ageRating)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("O título é obrigatório.");

        if (title.Trim().Length > TitleMaxLength)
            throw new DomainException($"O título deve ter no máximo {TitleMaxLength} caracteres.");

        if (synopsis?.Length > SynopsisMaxLength)
            throw new DomainException($"A sinopse deve ter no máximo {SynopsisMaxLength} caracteres.");

        if (!Enum.IsDefined(type))
            throw new DomainException("Tipo de conteúdo inválido.");

        var maxYear = DateTime.UtcNow.Year + 2;
        if (releaseYear < FirstReleaseYear || releaseYear > maxYear)
            throw new DomainException($"O ano de lançamento deve estar entre {FirstReleaseYear} e {maxYear}.");

        if (durationMinutes <= 0)
            throw new DomainException("A duração deve ser maior que zero.");

        if (!AllowedAgeRatings.Contains(ageRating))
            throw new DomainException("Classificação indicativa inválida. Valores aceitos: 0 (livre), 10, 12, 14, 16 e 18.");

        Title = title.Trim();
        Synopsis = string.IsNullOrWhiteSpace(synopsis) ? null : synopsis.Trim();
        Type = type;
        ReleaseYear = releaseYear;
        DurationMinutes = durationMinutes;
        AgeRating = ageRating;
    }
}
