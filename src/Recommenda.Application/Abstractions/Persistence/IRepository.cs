using System.Linq.Expressions;
using Recommenda.Domain.Common;

namespace Recommenda.Application.Abstractions.Persistence;

/// <summary>
/// Contrato genérico de acesso a dados para entidades que herdam de <see cref="BaseEntity"/>.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna uma página ordenada por <c>CreatedAt</c> e <c>Id</c>. O corte é feito no banco.
    /// </summary>
    Task<(IReadOnlyList<T> Items, int TotalItems)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);
}
