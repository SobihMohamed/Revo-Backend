using Revo.Application.Common.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Contracts.Notifications
{
    public interface INotificationStrategy
    {
        NotificationChannel Channel { get; }
        Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
    }
}
