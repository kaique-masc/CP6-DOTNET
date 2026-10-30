using Recommenda.Domain.Entities;
using Recommenda.Domain.Enums;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Domain.Tests.Entities;

public class ContentTests
{
    private static Content CreateValidContent() =>
        new("A Bruxa", "Terror de época", ContentType.Movie, 2015, 92, 14);

    [Fact]
    public void SetGenres_DuplicatedIds_KeepsDistinctGenres()
    {
        // Arrange
        var content = CreateValidContent();
        var terror = Guid.NewGuid();
        var drama = Guid.NewGuid();

        // Act
        content.SetGenres([terror, drama, terror]);

        // Assert
        Assert.Equal(2, content.ContentGenres.Count);
        Assert.All(content.ContentGenres, cg => Assert.Equal(content.Id, cg.ContentId));
    }

    [Fact]
    public void SetGenres_NewList_RemovesGenresNotInformed()
    {
        // Arrange
        var content = CreateValidContent();
        var terror = Guid.NewGuid();
        var drama = Guid.NewGuid();
        content.SetGenres([terror, drama]);

        // Act
        content.SetGenres([drama]);

        // Assert
        var single = Assert.Single(content.ContentGenres);
        Assert.Equal(drama, single.GenreId);
    }

    [Fact]
    public void SetGenres_EmptyList_ThrowsDomainException()
    {
        // Arrange
        var content = CreateValidContent();

        // Act
        var act = () => content.SetGenres([]);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(15)]
    [InlineData(21)]
    public void Constructor_InvalidAgeRating_ThrowsDomainException(int ageRating)
    {
        // Act
        var act = () => new Content("A Bruxa", null, ContentType.Movie, 2015, 92, ageRating);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(1800)]
    [InlineData(1887)]
    [InlineData(3000)]
    public void Constructor_InvalidReleaseYear_ThrowsDomainException(int year)
    {
        // Act
        var act = () => new Content("A Bruxa", null, ContentType.Movie, year, 92, 14);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("", 92)]
    [InlineData("   ", 92)]
    [InlineData("A Bruxa", 0)]
    [InlineData("A Bruxa", -10)]
    public void Constructor_MissingTitleOrInvalidDuration_ThrowsDomainException(string title, int duration)
    {
        // Act
        var act = () => new Content(title, null, ContentType.Movie, 2015, duration, 14);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void IsAdultOnly_AgeRating18_ReturnsTrue()
    {
        // Arrange
        var content = new Content("Filme adulto", null, ContentType.Movie, 2020, 100, 18);

        // Act
        var result = content.IsAdultOnly;

        // Assert
        Assert.True(result);
    }
}
