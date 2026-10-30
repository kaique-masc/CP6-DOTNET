using Recommenda.Domain.Common;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Domain.Entities;

public class Genre : BaseEntity
{
    public const int NameMaxLength = 60;
    public const int DescriptionMaxLength = 250;

    private readonly List<ContentGenre> _contentGenres = new();

    protected Genre()
    {
    }

    public Genre(string name, string? description = null)
    {
        SetValues(name, description);
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public IReadOnlyCollection<ContentGenre> ContentGenres => _contentGenres.AsReadOnly();

    public void Update(string name, string? description)
    {
        SetValues(name, description);
        Touch();
    }

    private void SetValues(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do gênero é obrigatório.");

        if (name.Trim().Length > NameMaxLength)
            throw new DomainException($"O nome do gênero deve ter no máximo {NameMaxLength} caracteres.");

        if (description?.Length > DescriptionMaxLength)
            throw new DomainException($"A descrição do gênero deve ter no máximo {DescriptionMaxLength} caracteres.");

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
