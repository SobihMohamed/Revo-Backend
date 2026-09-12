using Revo.Application.Contracts;
using Revo.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }
        public async Task<int> SaveChanges(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
