using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.DTOs.Users;
using Recommenda.Application.Mappings;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository users, IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _users.GetAllAsync(cancellationToken);
        return users.Select(u => u.ToResponse()).ToList();
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken)
                   ?? throw new ResourceNotFoundException("Usuário", id);

        return user.ToResponse();
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = new User(request.Name, request.Email, request.Password, request.BirthDate);

        if (await _users.EmailExistsAsync(user.Email, cancellationToken))
            throw new ConflictException($"Já existe um usuário cadastrado com o e-mail '{user.Email}'.");

        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }

    public async Task UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken)
                   ?? throw new ResourceNotFoundException("Usuário", id);

        user.Update(request.Name, request.BirthDate);
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken)
                   ?? throw new ResourceNotFoundException("Usuário", id);

        _users.Delete(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserConfigurationResponse> GetConfigurationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (!await _users.ExistsByIdAsync(userId, cancellationToken))
            throw new ResourceNotFoundException("Usuário", userId);

        var configuration = await _users.GetConfigurationAsync(userId, cancellationToken)
                            ?? throw new ResourceNotFoundException("Configuração do usuário", userId);

        return configuration.ToResponse();
    }

    public async Task<UserConfigurationResponse> SaveConfigurationAsync(Guid userId, UserConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _users.ExistsByIdAsync(userId, cancellationToken))
            throw new ResourceNotFoundException("Usuário", userId);

        var configuration = await _users.GetConfigurationAsync(userId, cancellationToken);
        if (configuration is null)
        {
            configuration = new UserConfiguration(userId, request.PreferredLanguage, request.AllowAdultContent,
                request.ReceiveNotifications, request.MaxRecommendations);
            await _users.AddConfigurationAsync(configuration, cancellationToken);
        }
        else
        {
            configuration.Update(request.PreferredLanguage, request.AllowAdultContent,
                request.ReceiveNotifications, request.MaxRecommendations);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return configuration.ToResponse();
    }
}
