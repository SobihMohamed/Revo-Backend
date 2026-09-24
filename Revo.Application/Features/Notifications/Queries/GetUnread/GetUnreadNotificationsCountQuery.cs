using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Notifications.Queries.GetUnread
{
    public record GetUnreadNotificationsCountQuery : IQuery<int>;
}
