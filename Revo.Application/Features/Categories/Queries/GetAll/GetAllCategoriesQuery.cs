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
    ) : IQuery<PaginationResponse<CategoryDto>>;    
}
