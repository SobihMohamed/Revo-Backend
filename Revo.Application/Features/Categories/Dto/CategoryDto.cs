using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Dto
{
    public record CategoryDto(
        Guid Id,
        string NameAr,
        string NameEn,
        string ImageUrl,
        int OrderIndex,
        int PortfolioItemsCount
    );
}
