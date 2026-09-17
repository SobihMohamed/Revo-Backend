using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Dto
{
    public record PortfolioItemSnippetDto(
        Guid Id,
        string CaptionAr,
        string CaptionEn,
        int OrderIndex,
        string MainImageUrl
    );
}
