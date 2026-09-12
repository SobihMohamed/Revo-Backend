using Revo.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Entities
{
    public class PortfolioItem : AuditableEntity<Guid>, ISoftDeletable
    {
        public string CaptionAr { get; set; } = string.Empty;
        public string CaptionEn { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<PortfolioMedia> MediaItems { get; set; } = new List<PortfolioMedia>();
        public bool IsDeleted {  get; set; } = false;
    }
}
