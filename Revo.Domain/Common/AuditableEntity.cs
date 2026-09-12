using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Common
{
    public class AuditableEntity<TId> : BaseEntity<TId>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
