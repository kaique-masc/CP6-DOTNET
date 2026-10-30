using Recommenda.Domain.Entities;
using Recommenda.Domain.Enums;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Domain.Tests.Entities;

public class UserTests
{
    private static DateOnly YearsAgo(int years) => DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-years);

    [Fact]
    public void Constructor_ValidData_CreatesUserWithHashedPassword()
    {
        // Arrange
        var birthDate = YearsAgo(25);

        // Act
        var user = new User("Ana Souza", "Ana@Example.com", "senha1234", birthDate);

        // Assert
        Assert.Equal("Ana Souza", user.Name);
        Assert.Equal("ana@example.com", user.Email);
        Assert.Equal(UserRole.User, user.Role);
        Assert.Equal(25, user.Age);
        Assert.NotEqual("senha1234", user.Password);
        Assert.StartsWith("PBKDF2$", user.Password);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(12)]
    public void Constructor_AgeBelowMinimum_ThrowsDomainException(int age)
    {
        // Arrange
        var birthDate = YearsAgo(age);

        // Act
        var act = () => new User("Ana Souza", "ana@example.com", "senha1234", birthDate);

        // Assert
        var exception = Assert.Throws<DomainException>(act);
        Assert.Contains("13 anos", exception.Message);
    }

    [Fact]
    public void Constructor_BirthDateInFuture_ThrowsDomainException()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        // Act
        var act = () => new User("Ana Souza", "ana@example.com", "senha1234", birthDate);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc123")]
    [InlineData("somenteletras")]
    [InlineData("12345678")]
    public void Constructor_InvalidPassword_ThrowsDomainException(string password)
    {
        // Arrange
        var birthDate = YearsAgo(20);

        // Act
        var act = () => new User("Ana Souza", "ana@example.com", password, birthDate);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ana")]
    [InlineData("ana@")]
    [InlineData("ana souza@example.com")]
    public void Constructor_InvalidEmail_ThrowsDomainException(string email)
    {
        // Arrange
        var birthDate = YearsAgo(20);

        // Act
        var act = () => new User("Ana Souza", email, "senha1234", birthDate);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var user = new User("Ana Souza", "ana@example.com", "senha1234", YearsAgo(20));

        // Act
        var result = user.VerifyPassword("senha1234");

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("senha12345")]
    [InlineData("SENHA1234")]
    public void VerifyPassword_WrongPassword_ReturnsFalse(string attempt)
    {
        // Arrange
        var user = new User("Ana Souza", "ana@example.com", "senha1234", YearsAgo(20));

        // Act
        var result = user.VerifyPassword(attempt);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ChangePassword_WrongCurrentPassword_ThrowsDomainException()
    {
        // Arrange
        var user = new User("Ana Souza", "ana@example.com", "senha1234", YearsAgo(20));

        // Act
        var act = () => user.ChangePassword("outrasenha1", "novasenha99");

        // Assert
        Assert.Throws<DomainException>(act);
        Assert.True(user.VerifyPassword("senha1234"));
    }

    [Theory]
    [InlineData(2000, 5, 10, 2026, 5, 9, 25)]
    [InlineData(2000, 5, 10, 2026, 5, 10, 26)]
    [InlineData(2008, 2, 29, 2026, 2, 28, 17)]
    public void CalculateAge_GivenDates_ReturnsCompletedYears(int by, int bm, int bd, int ty, int tm, int td, int expected)
    {
        // Act
        var age = User.CalculateAge(new DateOnly(by, bm, bd), new DateOnly(ty, tm, td));

        // Assert
        Assert.Equal(expected, age);
    }
}
