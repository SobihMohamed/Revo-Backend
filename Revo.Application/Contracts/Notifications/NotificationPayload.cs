using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Contracts.Notifications
{
    public class NotificationPayload
    {
        public Guid Id { get; set; }
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string MessageAr { get; set; } = string.Empty;
        public string MessageEn { get; set; } = string.Empty;
        public string? TargetUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
