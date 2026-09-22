using Revo.Application.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Implementations.Notifications
{
    public class NotificationHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
