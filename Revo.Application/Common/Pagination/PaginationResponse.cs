using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Common.Pagination
{
    public class PaginationResponse<TData>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IReadOnlyList<TData> Data { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public  PaginationResponse(int index, int size, int total, IReadOnlyList<TData> data)
        {
            PageIndex = index;
            PageSize = size;
            TotalCount = total; 
            Data = data;
        }
    }
}
