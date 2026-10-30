using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Domain.Tests.Entities;

public class UserConfigurationTests
{
    [Fact]
    public void Constructor_ValidValues_CreatesConfiguration()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var configuration = new UserConfiguration(userId, " pt-BR ", false, true, 15);

        // Assert
        Assert.Equal(userId, configuration.UserId);
        Assert.Equal("pt-BR", configuration.PreferredLanguage);
        Assert.Equal(15, configuration.MaxRecommendations);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    [InlineData(-5)]
    public void Constructor_MaxRecommendationsOutOfRange_ThrowsDomainException(int max)
    {
        // Act
        var act = () => new UserConfiguration(Guid.NewGuid(), "pt-BR", false, true, max);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Constructor_EmptyUserId_ThrowsDomainException()
    {
        // Act
        var act = () => new UserConfiguration(Guid.Empty, "pt-BR", false, true, 10);

        // Assert
        Assert.Throws<DomainException>(act);
    }
}
