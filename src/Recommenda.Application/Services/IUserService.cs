using Recommenda.Application.DTOs.Users;

namespace Recommenda.Application.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserConfigurationResponse> GetConfigurationAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserConfigurationResponse> SaveConfigurationAsync(Guid userId, UserConfigurationRequest request, CancellationToken cancellationToken = default);
}
