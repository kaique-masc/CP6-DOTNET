using Moq;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.DTOs.Ratings;
using Recommenda.Application.Services;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Tests.Services;

public class RatingServiceTests
{
    private readonly Mock<IRatingRepository> _ratings = new();
    private readonly Mock<IRepository<User>> _users = new();
    private readonly Mock<IRepository<Content>> _contents = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly RatingService _service;

    public RatingServiceTests()
    {
        _service = new RatingService(_ratings.Object, _users.Object, _contents.Object, _unitOfWork.Object);
    }

    private static CreateRatingRequest Request() => new()
    {
        UserId = Guid.NewGuid(),
        ContentId = Guid.NewGuid(),
        Score = 5,
        Comment = "Excelente"
    };

    [Fact]
    public async Task CreateAsync_UserDoesNotExist_ThrowsResourceNotFoundAndNeverAdds()
    {
        // Arrange
        var request = Request();
        _users.Setup(u => u.ExistsByIdAsync(request.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(act);
        Assert.Equal("Usuário", exception.Resource);
        _ratings.Verify(r => r.AddAsync(It.IsAny<Rating>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ContentDoesNotExist_ThrowsResourceNotFoundAndNeverAdds()
    {
        // Arrange
        var request = Request();
        _users.Setup(u => u.ExistsByIdAsync(request.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _contents.Setup(c => c.ExistsByIdAsync(request.ContentId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(act);
        Assert.Equal("Conteúdo", exception.Resource);
        _ratings.Verify(r => r.AddAsync(It.IsAny<Rating>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UserAlreadyRatedContent_ThrowsConflictAndNeverAdds()
    {
        // Arrange
        var request = Request();
        _users.Setup(u => u.ExistsByIdAsync(request.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _contents.Setup(c => c.ExistsByIdAsync(request.ContentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _ratings.Setup(r => r.ExistsForUserAndContentAsync(request.UserId, request.ContentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
        _ratings.Verify(r => r.AddAsync(It.IsAny<Rating>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsOnce()
    {
        // Arrange
        var request = Request();
        _users.Setup(u => u.ExistsByIdAsync(request.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _contents.Setup(c => c.ExistsByIdAsync(request.ContentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.Equal(5, result.Score);
        _ratings.Verify(r => r.AddAsync(It.Is<Rating>(x => x.UserId == request.UserId && x.ContentId == request.ContentId), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
