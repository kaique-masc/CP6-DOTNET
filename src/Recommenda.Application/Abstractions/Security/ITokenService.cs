using Recommenda.Domain.Entities;

namespace Recommenda.Application.Abstractions.Security;

public interface ITokenService
{
    AccessToken Generate(User user);
}

public sealed record AccessToken(string Token, DateTime ExpiresAt);
