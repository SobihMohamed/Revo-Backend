using Revo.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Entities
{
    public class Admin : AuditableEntity<Guid>, ISoftDeletable
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}
