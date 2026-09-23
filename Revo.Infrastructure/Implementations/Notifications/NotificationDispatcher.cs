using Revo.Application.Common.Notifications;
using Revo.Application.Contracts.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Implementations.Notifications
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IEnumerable<INotificationStrategy> _strategies;

        public NotificationDispatcher(IEnumerable<INotificationStrategy> strategies)
        {
            _strategies = strategies;
        }

        public async Task DispatchAsync(NotificationMessage message, CancellationToken cancellationToken = default)
        {
            if (message.Channels == null || !message.Channels.Any())
                return;

            var strategiesToExecute = _strategies
                .Where(strategy => message.Channels.Contains(strategy.Channel));

            var tasks = strategiesToExecute.Select(strategy => strategy.SendAsync(message, cancellationToken));

            await Task.WhenAll(tasks);
        }
    }
}