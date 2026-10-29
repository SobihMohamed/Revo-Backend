using Revo.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Dto
{
    public record CategoryDetailsDto(
        Guid Id,
        string NameAr,
        string NameEn,
        string ImageUrl,
        int OrderIndex,
        PaginationResponse<PortfolioItemSnippetDto> Items
    );
}
