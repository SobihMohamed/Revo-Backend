using Revo.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Queries.Helper
{
    public record PortfolioItemSpecParams : PaginationParams
    {
        public Guid? CategoryId { get; init; }

        private string? _search;
        public string? Search
        {
            get => _search;
            init => _search = value?.ToLower();
        }

        public PortfolioSortOption? Sort { get; init; }
    }
}
