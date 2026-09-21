using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Dto
{
    public class PortfolioItemListDto
    {
        public Guid Id { get; set; }
        public string CaptionAr { get; set; } = string.Empty;
        public string CaptionEn { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryNameAr { get; set; } = string.Empty;
        public string CategoryNameEn { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }
        public MediaType? ThumbnailType { get; set; }
    }
}
