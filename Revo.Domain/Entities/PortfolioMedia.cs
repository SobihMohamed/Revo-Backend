using Revo.Domain.Common;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Entities
{
    public class PortfolioMedia : AuditableEntity<Guid>, ISoftDeletable
    {
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaPublicId { get; set; } = string.Empty; 
        public string? CoverImagePublicId { get; set; } 
        public string? CoverImageUrl { get; set; } // Optional cover image for video media
        public MediaType Type { get; set; }
        public int OrderIndex { get; set; }

        public Guid PortfolioItemId { get; set; }
        public PortfolioItem PortfolioItem { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;    
    }
}
