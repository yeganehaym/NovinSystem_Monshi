using Microsoft.EntityFrameworkCore;

namespace Monshi.Data.SqlServer;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}