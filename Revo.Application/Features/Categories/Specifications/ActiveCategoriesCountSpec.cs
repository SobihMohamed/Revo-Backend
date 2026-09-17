using Ardalis.Specification;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Specifications
{
    public class ActiveCategoriesCountSpec : Specification<Category>
    {
        public ActiveCategoriesCountSpec()
        {
            Query
                .Where(c => !c.IsDeleted);
        }
    }
}
