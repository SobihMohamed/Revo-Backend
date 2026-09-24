using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Notifications.Commands.MarkAsRead
{
    public record MarkNotificationAsReadCommand(Guid Id) : ICommand<bool>;
}
