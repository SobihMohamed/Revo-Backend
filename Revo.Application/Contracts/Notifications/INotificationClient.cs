using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Contracts.Notifications
{
    public interface INotificationClient
    {
        Task ReciveNotification(NotificationPayload payload);
    }
}
