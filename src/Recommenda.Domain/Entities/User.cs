using System.Net.Mail;
using Recommenda.Domain.Common;
using Recommenda.Domain.Enums;
using Recommenda.Domain.Exceptions;
using Recommenda.Domain.Security;

namespace Recommenda.Domain.Entities;

public class User : BaseEntity
{
    public const int NameMaxLength = 120;
    public const int EmailMaxLength = 180;
    public const int PasswordMinLength = 8;
    public const int MinimumAge = 13;

    private readonly List<Rating> _ratings = new();

    protected User()
    {
    }

    public User(string name, string email, string password, DateOnly birthDate, UserRole role = UserRole.User)
    {
        SetName(name);
        SetEmail(email);
        SetBirthDate(birthDate);
        SetPassword(password);
        Role = role;
    }

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public UserRole Role { get; private set; } = UserRole.User;

    public UserConfiguration? Configuration { get; private set; }
    public IReadOnlyCollection<Rating> Ratings => _ratings.AsReadOnly();

    public int Age => CalculateAge(BirthDate, DateOnly.FromDateTime(DateTime.UtcNow));

    public void Update(string name, DateOnly birthDate)
    {
        SetName(name);
        SetBirthDate(birthDate);
        Touch();
    }

    public void ChangePassword(string currentPassword, string newPassword)
    {
        if (!VerifyPassword(currentPassword))
            throw new DomainException("A senha atual informada não confere.");

        SetPassword(newPassword);
        Touch();
    }

    public bool VerifyPassword(string password) => PasswordHasher.Verify(password, Password);

    public void ChangeRole(UserRole role)
    {
        if (!Enum.IsDefined(role))
            throw new DomainException("Papel de usuário inválido.");

        Role = role;
        Touch();
    }

    public static int CalculateAge(DateOnly birthDate, DateOnly today)
    {
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age))
            age--;
        return age;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do usuário é obrigatório.");

        name = name.Trim();
        if (name.Length > NameMaxLength)
            throw new DomainException($"O nome do usuário deve ter no máximo {NameMaxLength} caracteres.");

        Name = name;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("O e-mail é obrigatório.");

        email = email.Trim().ToLowerInvariant();
        if (email.Length > EmailMaxLength || !MailAddress.TryCreate(email, out var parsed) || parsed.Address != email)
            throw new DomainException("O e-mail informado é inválido.");

        Email = email;
    }

    private void SetBirthDate(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (birthDate > today)
            throw new DomainException("A data de nascimento não pode estar no futuro.");

        if (CalculateAge(birthDate, today) < MinimumAge)
            throw new DomainException($"O usuário deve ter pelo menos {MinimumAge} anos.");

        BirthDate = birthDate;
    }

    private void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < PasswordMinLength)
            throw new DomainException($"A senha deve ter pelo menos {PasswordMinLength} caracteres.");

        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            throw new DomainException("A senha deve conter letras e números.");

        Password = PasswordHasher.Hash(password);
    }
}
