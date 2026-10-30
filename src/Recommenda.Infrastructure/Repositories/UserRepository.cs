using Microsoft.EntityFrameworkCore;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Domain.Entities;
using Recommenda.Infrastructure.Persistence;

namespace Recommenda.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(RecommendaContext context) : base(context)
    {
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => DbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(u => u.Email == email, cancellationToken);

    public Task<UserConfiguration?> GetConfigurationAsync(Guid userId, CancellationToken cancellationToken = default)
        => Context.UserConfigurations.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public async Task AddConfigurationAsync(UserConfiguration configuration, CancellationToken cancellationToken = default)
        => await Context.UserConfigurations.AddAsync(configuration, cancellationToken);
}
