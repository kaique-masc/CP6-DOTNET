namespace Recommenda.Domain.Entities;

public class ContentGenre
{
    protected ContentGenre()
    {
    }

    public ContentGenre(Guid contentId, Guid genreId)
    {
        ContentId = contentId;
        GenreId = genreId;
        AssignedAt = DateTime.UtcNow;
    }

    public Guid ContentId { get; private set; }
    public Guid GenreId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public Content? Content { get; private set; }
    public Genre? Genre { get; private set; }
}
