using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Dto
{
    public class PortfolioItemDetailsDto
    {
        public Guid Id { get; set; }
        public string CaptionAr { get; set; } = string.Empty;
        public string CaptionEn { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryNameAr { get; set; } = string.Empty;
        public string CategoryNameEn { get; set; } = string.Empty;
        public List<PortfolioMediaDto> MediaItems { get; set; } = new();
    }
    public class PortfolioMediaDto
    {
        public Guid Id { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public MediaType Type { get; set; }
        public int OrderIndex { get; set; }
    }
}
