using Ardalis.Specification;
using Revo.Application.Features.Categories.Dto;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Specifications
{
    public class ActiveCategoriesSpec : Specification<Category, CategoryDto>
    {
        public ActiveCategoriesSpec(int pageIndex, int pageSize)
        {
            Query
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.OrderIndex);

            Query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            Query.Select(c => new CategoryDto(
                c.Id,
                c.NameAr,
                c.NameEn,
                c.ImageUrl,
                c.OrderIndex,
                c.PortfolioItems.Count() 
            ));
        }
    }
}
