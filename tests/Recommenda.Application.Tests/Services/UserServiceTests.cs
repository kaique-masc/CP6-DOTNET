using Moq;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.DTOs.Users;
using Recommenda.Application.Services;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_users.Object, _unitOfWork.Object);
    }

    private static CreateUserRequest Request() => new()
    {
        Name = "Ana Souza",
        Email = "ana@example.com",
        Password = "senha1234",
        BirthDate = new DateOnly(2000, 5, 10)
    };

    [Fact]
    public async Task CreateAsync_EmailAlreadyExists_ThrowsConflictAndNeverAdds()
    {
        // Arrange
        _users.Setup(u => u.EmailExistsAsync("ana@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(Request());

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
        _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsOnceWithUserRole()
    {
        // Arrange
        _users.Setup(u => u.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _service.CreateAsync(Request());

        // Assert
        Assert.Equal("User", result.Role);
        _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveConfigurationAsync_UserDoesNotExist_ThrowsResourceNotFoundAndNeverAdds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _users.Setup(u => u.ExistsByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var act = () => _service.SaveConfigurationAsync(userId, new UserConfigurationRequest());

        // Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(act);
        _users.Verify(u => u.AddConfigurationAsync(It.IsAny<UserConfiguration>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
