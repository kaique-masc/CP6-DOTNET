using Recommenda.Domain.Entities;

namespace Recommenda.Application.Abstractions.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<UserConfiguration?> GetConfigurationAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddConfigurationAsync(UserConfiguration configuration, CancellationToken cancellationToken = default);
}
