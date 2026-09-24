using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Notifications.Specification
{
    public class GetUnreadNotificationsSpec : Specification<Domain.Entities.Notification>
    {
        public GetUnreadNotificationsSpec()
        {
            Query.Where(x => !x.IsRead);
        }
    }
}
