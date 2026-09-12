using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChanges(CancellationToken cancellationToken = default);
    }
}
