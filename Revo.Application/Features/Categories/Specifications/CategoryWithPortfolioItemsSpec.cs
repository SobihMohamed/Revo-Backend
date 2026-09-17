using Ardalis.Specification;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Specifications
{
    public class CategoryWithPortfolioItemsSpec : Specification<Category>, ISingleResultSpecification<Category>
    {
        public CategoryWithPortfolioItemsSpec(Guid categoryId)
        {
            Query
                .Where(c => c.Id == categoryId)
                .Include(c => c.PortfolioItems); 
        }
    }
}
