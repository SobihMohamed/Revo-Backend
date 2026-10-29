using Revo.Application.Common.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Contracts.Notifications
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync(NotificationMessage message, CancellationToken cancellationToken = default);
    }
}
