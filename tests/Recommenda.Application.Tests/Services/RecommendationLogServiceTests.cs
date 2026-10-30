using Moq;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.DTOs.RecommendationLogs;
using Recommenda.Application.Exceptions;
using Recommenda.Application.Services;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Tests.Services;

public class RecommendationLogServiceTests
{
    private readonly Mock<IRecommendationLogRepository> _logs = new();
    private readonly RecommendationLogService _service;

    public RecommendationLogServiceTests()
    {
        _service = new RecommendationLogService(_logs.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_InsertsOnceWithNewId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateRecommendationLogRequest
        {
            UserId = userId,
            Query = "filme de terror curto",
            Titles = ["A Bruxa", "Hereditário"]
        };

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(userId, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(2, result.Titles.Count);
        _logs.Verify(l => l.InsertAsync(It.IsAny<RecommendationLog>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_EmptyQuery_ThrowsValidationAndNeverInserts(string? query)
    {
        // Arrange
        var request = new CreateRecommendationLogRequest { UserId = Guid.NewGuid(), Query = query, Titles = ["A Bruxa"] };

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _logs.Verify(l => l.InsertAsync(It.IsAny<RecommendationLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    public static TheoryData<List<string>?> InvalidTitles => new()
    {
        null,
        new List<string>(),
        new List<string> { "A Bruxa", " " },
        new List<string> { "" }
    };

    [Theory]
    [MemberData(nameof(InvalidTitles))]
    public async Task CreateAsync_InvalidTitles_ThrowsValidationAndNeverInserts(List<string>? titles)
    {
        // Arrange
        var request = new CreateRecommendationLogRequest { UserId = Guid.NewGuid(), Query = "terror", Titles = titles };

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _logs.Verify(l => l.InsertAsync(It.IsAny<RecommendationLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_EmptyUserId_ThrowsValidationAndNeverInserts()
    {
        // Arrange
        var request = new CreateRecommendationLogRequest { UserId = Guid.Empty, Query = "terror", Titles = ["A Bruxa"] };

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _logs.Verify(l => l.InsertAsync(It.IsAny<RecommendationLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByUserAsync_EmptyUserId_ThrowsValidationException()
    {
        // Act
        var act = () => _service.GetByUserAsync(Guid.Empty);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _logs.Verify(l => l.GetByUserAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_LogDoesNotExist_ThrowsResourceNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _logs.Setup(l => l.UpdateAsync(id, It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var request = new UpdateRecommendationLogRequest { Query = "terror dublado", Titles = ["A Bruxa"] };

        // Act
        var act = () => _service.UpdateAsync(id, request);

        // Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(act);
    }
}
