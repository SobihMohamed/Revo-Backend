using Ardalis.Specification;
using Revo.Application.Features.Notifications.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Notifications.Specification
{
    public class GetNotificationsCountSpec : Specification<Domain.Entities.Notification>
    {
        public GetNotificationsCountSpec(NotificationSpecParams specParams)
        {
            Query.Where(x => !specParams.IsRead.HasValue || x.IsRead == specParams.IsRead.Value);
        }
    }
}
