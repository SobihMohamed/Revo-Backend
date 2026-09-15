using Ardalis.Specification.EntityFrameworkCore;
using Revo.Application.Contracts.Repositories;
using Revo.Infrastructure.Database;

namespace Revo.Infrastructure.Repos
{
    public class GenericRepo<T> : RepositoryBase<T> , IGenericRepo<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        public GenericRepo(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }
    }
}
