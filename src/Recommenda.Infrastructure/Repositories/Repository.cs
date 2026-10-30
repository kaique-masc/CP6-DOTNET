using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Domain.Common;
using Recommenda.Infrastructure.Persistence;

namespace Recommenda.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly RecommendaContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(RecommendaContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .OrderBy(e => e.CreatedAt)
            .ThenBy(e => e.Id)
            .ToListAsync(cancellationToken);

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(e => e.Id == id, cancellationToken);

    public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(predicate, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public virtual async Task<(IReadOnlyList<T> Items, int TotalItems)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(e => e.CreatedAt)
            .ThenBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public virtual void Update(T entity)
    {
        // entidades carregadas pelo próprio contexto já estão rastreadas
        if (Context.Entry(entity).State == EntityState.Detached)
            DbSet.Update(entity);
    }

    public virtual void Delete(T entity) => DbSet.Remove(entity);
}
