using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Specification
{
    public class GetPortofolioWithMediaSpec : Specification<Domain.Entities.PortfolioItem>
    {
        public GetPortofolioWithMediaSpec(Guid id)
        {
            Query.Where(x => x.Id == id)
                 .Include(x => x.MediaItems);
        }
    }
}
