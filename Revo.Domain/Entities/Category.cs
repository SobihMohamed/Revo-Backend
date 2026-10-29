using Revo.Domain.Common;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Entities
{
    public class Category : AuditableEntity<Guid>, ISoftDeletable
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ImagePublicId { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public ICollection<PortfolioItem> PortfolioItems { get; set; } = new List<PortfolioItem>();
        public bool IsDeleted { get; set; } = false;
    }
}
