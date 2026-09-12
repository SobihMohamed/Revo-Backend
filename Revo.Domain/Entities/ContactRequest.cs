using Revo.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Entities
{
    public class ContactRequest : AuditableEntity<Guid>, ISoftDeletable
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Message { get; set; }
        public bool IsRead { get; set; } = false;
        public Guid? ServiceId { get; set; }
        public Service? Service { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
