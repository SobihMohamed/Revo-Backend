using Microsoft.Extensions.Logging;
using Revo.Application.Common.Notifications;
using Revo.Application.Contracts.Notifications;

namespace Revo.Infrastructure.Implementations.Notifications
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IEnumerable<INotificationStrategy> _strategies;
        private readonly ILogger<NotificationDispatcher> _logger;

        public NotificationDispatcher(
            IEnumerable<INotificationStrategy> strategies,
            ILogger<NotificationDispatcher> logger)
        {
            _strategies = strategies;
            _logger = logger;
        }

        public async Task DispatchAsync(NotificationMessage message, CancellationToken cancellationToken = default)
        {
            if (message.Channels == null || !message.Channels.Any())
                return;

            var strategiesToExecute = _strategies
                .Where(strategy => message.Channels.Contains(strategy.Channel));

            var tasks = strategiesToExecute.Select(async strategy =>
            {
                try
                {
                    await strategy.SendAsync(message, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to send notification via {strategy.Channel}. Error: {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}