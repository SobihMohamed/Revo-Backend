using Revo.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Services.Queries.Helper
{
    public record ServiceSpecParams : PaginationParams
    {
        public ServiceSortOption Sort { get; set; } = ServiceSortOption.OrderAsc;
        public string? Search { get; set; }
    }
}
