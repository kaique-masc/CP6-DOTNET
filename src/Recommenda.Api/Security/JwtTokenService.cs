using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Recommenda.Application.Abstractions.Security;
using Recommenda.Domain.Entities;
using JwtClaims = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Recommenda.Api.Security;

public class JwtTokenService : ITokenService
{
    public const string RoleClaim = "role";

    private readonly JwtOptions _options;
    private readonly JsonWebTokenHandler _handler = new();

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessToken Generate(User user)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.ExpirationMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [JwtClaims.Sub] = user.Id.ToString(),
                [JwtClaims.Email] = user.Email,
                [JwtClaims.Name] = user.Name,
                [RoleClaim] = user.Role.ToString(),
                [JwtClaims.Jti] = Guid.NewGuid().ToString()
            }
        };

        return new AccessToken(_handler.CreateToken(descriptor), expiresAt);
    }
}
