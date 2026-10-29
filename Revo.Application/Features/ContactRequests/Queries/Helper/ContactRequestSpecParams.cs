using Revo.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.ContactRequests.Queries.Helper
{
    public record ContactRequestSpecParams : PaginationParams
    {
        private string? _search;
        public string? Search
        {
            get => _search;
            init => _search = value?.ToLower();
        }
        public bool? IsRead { get; init; }
        public ContactRequestSortOptions? Sort { get; init; }
    } 
}
