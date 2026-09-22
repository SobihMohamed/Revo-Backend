using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Common.Notifications
{
    public class NotificationMessage
    {
        public Guid NotificationId { get; set; }
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string MessageAr { get; set; } = string.Empty;
        public string MessageEn { get; set; } = string.Empty;
        public string? TargetUrl { get; set; }
        public string? WhatsAppRecipient { get; set; }

        public List<NotificationChannel> Channels { get; set; } = new();
    }
}
