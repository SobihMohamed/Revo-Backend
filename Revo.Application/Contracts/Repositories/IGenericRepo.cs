using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Contracts.Repositories
{
    public interface IGenericRepo<T> : IRepositoryBase<T> where T : class
    {
    }
}
