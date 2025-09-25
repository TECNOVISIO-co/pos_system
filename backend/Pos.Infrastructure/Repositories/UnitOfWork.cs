using Pos.Infrastructure.Data;

namespace Pos.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PosDbContext _context;

    public UnitOfWork(PosDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
