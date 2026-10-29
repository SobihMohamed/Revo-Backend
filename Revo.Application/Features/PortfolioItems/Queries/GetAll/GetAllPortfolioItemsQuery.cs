using Revo.Application.Abstraction.Caching;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.PortfolioItems.Dto;
using Revo.Application.Features.PortfolioItems.Queries.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Queries.GetAll
{
    public record GetAllPortfolioItemsQuery(PortfolioItemSpecParams SpecParams)
          : IQuery<PaginationResponse<PortfolioItemListDto>>, ICacheableQuery
    {
        // Cache group for portfolio items
        public string CacheGroup => "PortfolioItems";
        // Cache key for portfolio items based on search, category, sort, page index, and page size
        public string CacheKey => !string.IsNullOrWhiteSpace(SpecParams.Search)
            ? null!
            : $"Cat_{SpecParams.CategoryId?.ToString() ?? "All"}" +
            $"_Sort_{SpecParams.Sort?.ToString() ?? "Default"}" +
            $"_Page_{SpecParams.PageIndex}_Size_{SpecParams.PageSize}";

        public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
    }
}
