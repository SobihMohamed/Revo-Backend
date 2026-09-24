using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Notifications.Dtos
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string MessageAr { get; set; } = string.Empty;
        public string MessageEn { get; set; } = string.Empty;
        public string? TargetUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
