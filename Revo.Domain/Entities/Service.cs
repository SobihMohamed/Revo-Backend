using Revo.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Entities
{
    public class Service : AuditableEntity<Guid>, ISoftDeletable
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}