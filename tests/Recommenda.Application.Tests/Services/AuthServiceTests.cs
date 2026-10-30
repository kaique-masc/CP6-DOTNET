using Moq;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.Abstractions.Security;
using Recommenda.Application.DTOs.Auth;
using Recommenda.Application.Services;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Enums;

namespace Recommenda.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly AuthService _service;
    private readonly User _user = new("Ana Souza", "ana@example.com", "senha1234", new DateOnly(2000, 5, 10), UserRole.Admin);

    public AuthServiceTests()
    {
        _service = new AuthService(_users.Object, _tokens.Object);
        _users.Setup(u => u.GetByEmailAsync("ana@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(_user);
        _tokens.Setup(t => t.Generate(It.IsAny<User>()))
            .Returns(new AccessToken("token-gerado", new DateTime(2026, 10, 19, 3, 0, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task LoginAsync_CorrectPassword_ReturnsTokenWithEmailAndRole()
    {
        // Arrange
        var request = new LoginRequest { Email = "ANA@example.com", Password = "senha1234" };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("token-gerado", result.AccessToken);
        Assert.Equal("ana@example.com", result.Email);
        Assert.Equal("Admin", result.Role);
        _tokens.Verify(t => t.Generate(_user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        // Arrange
        var request = new LoginRequest { Email = "ana@example.com", Password = "senhaerrada1" };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.Null(result);
        _tokens.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsNull()
    {
        // Arrange
        var request = new LoginRequest { Email = "naoexiste@example.com", Password = "senha1234" };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.Null(result);
        _tokens.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
    }
}
