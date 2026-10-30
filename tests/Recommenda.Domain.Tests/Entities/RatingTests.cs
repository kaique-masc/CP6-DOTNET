using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Domain.Tests.Entities;

public class RatingTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Constructor_ScoreWithinRange_CreatesRating(int score)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var contentId = Guid.NewGuid();

        // Act
        var rating = new Rating(userId, contentId, score, "  Muito bom  ");

        // Assert
        Assert.Equal(score, rating.Score);
        Assert.Equal(userId, rating.UserId);
        Assert.Equal(contentId, rating.ContentId);
        Assert.Equal("Muito bom", rating.Comment);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    [InlineData(10)]
    public void Constructor_ScoreOutOfRange_ThrowsDomainException(int score)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var contentId = Guid.NewGuid();

        // Act
        var act = () => new Rating(userId, contentId, score);

        // Assert
        var exception = Assert.Throws<DomainException>(act);
        Assert.Contains("entre 1 e 5", exception.Message);
    }

    [Fact]
    public void Constructor_EmptyUserId_ThrowsDomainException()
    {
        // Act
        var act = () => new Rating(Guid.Empty, Guid.NewGuid(), 4);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Update_ScoreOutOfRange_KeepsPreviousScore()
    {
        // Arrange
        var rating = new Rating(Guid.NewGuid(), Guid.NewGuid(), 4);

        // Act
        var act = () => rating.Update(7, null);

        // Assert
        Assert.Throws<DomainException>(act);
        Assert.Equal(4, rating.Score);
    }
}
