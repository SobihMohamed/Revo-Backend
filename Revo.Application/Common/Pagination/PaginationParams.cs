using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Common.Pagination
{
    public record PaginationParams
    {
        private const int MaxPageSize = 10;
        private int _pageSize = 10;

        public int PageIndex { get; init; } = 1;

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
