using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private const int DuplicateEntry = 1062;
    private const int RowIsReferenced = 1451;

    private readonly RecommendaContext _context;

    public UnitOfWork(RecommendaContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: DuplicateEntry })
        {
            throw new ConflictException("Já existe um registro com os mesmos dados únicos.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: RowIsReferenced })
        {
            throw new ConflictException("O registro está em uso por outros dados e não pode ser removido.");
        }
    }
}
