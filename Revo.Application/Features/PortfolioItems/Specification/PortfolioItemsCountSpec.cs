using Ardalis.Specification;
using Revo.Application.Features.PortfolioItems.Queries.Helper;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Specification
{
    public class PortfolioItemsCountSpec : Specification<PortfolioItem>
    {
        public PortfolioItemsCountSpec(PortfolioItemSpecParams portfolioItemSpec)
        {
            Query.Where(p =>
                 (!portfolioItemSpec.CategoryId.HasValue || p.CategoryId == portfolioItemSpec.CategoryId) &&
                 (string.IsNullOrEmpty(portfolioItemSpec.Search) ||
                  p.CaptionAr.ToLower().Contains(portfolioItemSpec.Search) ||
                  p.CaptionEn.ToLower().Contains(portfolioItemSpec.Search))
             );
        }
    }
}
