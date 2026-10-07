using Revo.Application.Abstraction.Caching;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.Categories.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Queries.GetAll
{
    public record GetAllCategoriesQuery(
        int PageIndex,
        int PageSize
    ) : IQuery<PaginationResponse<CategoryDto>>, ICacheableQuery
    {
        public string CacheGroup => "Categories";
        public string CacheKey => $"Page_{PageIndex}_Size_{PageSize}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
    }
}
