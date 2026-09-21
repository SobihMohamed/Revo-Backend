using Ardalis.Specification;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Specifications
{
    public class PortfolioItemsCountByCategoryIdSpec : Specification<PortfolioItem>
    {
        public PortfolioItemsCountByCategoryIdSpec(Guid categoryId)
        {
            Query.Where(p => p.CategoryId == categoryId);
        }
    }
}
