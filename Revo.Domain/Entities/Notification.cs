using Revo.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Entities
{
    public class Notification : AuditableEntity<Guid>
    {
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string MessageAr { get; set; } = string.Empty;
        public string MessageEn { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;

        public string? TargetUrl { get; set; } // Optional URL to navigate when the notification is clicked
    }
}
