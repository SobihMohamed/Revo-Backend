using Microsoft.AspNetCore.SignalR;
using Revo.Application.Common.Notifications;
using Revo.Application.Contracts.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Implementations.Notifications.Strategies
{
    public class PushNotificationStrategy : INotificationStrategy
    {
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        public PushNotificationStrategy(IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _hubContext = hubContext;
        }
        public NotificationChannel Channel => NotificationChannel.Push;
        public async Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
        {
            var payload = new NotificationPayload
            {
                Id = message.NotificationId,
                TitleAr = message.TitleAr,
                TitleEn = message.TitleEn,
                MessageAr = message.MessageAr,
                MessageEn = message.MessageEn,
                TargetUrl = message.TargetUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _hubContext.Clients.Group("Admins").ReciveNotification(payload);
        }
    }
}
