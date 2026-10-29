using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Notifications.Specification;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Notifications.Queries.GetUnread
{
    public class GetUnreadNotificationsCountQueryHandler : IQueryHandler<GetUnreadNotificationsCountQuery, int>
    {
        private readonly IGenericRepo<Notification> _notificationRepo;

        public GetUnreadNotificationsCountQueryHandler(IGenericRepo<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<Result<int>> Handle(
            GetUnreadNotificationsCountQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new GetUnreadNotificationsSpec();

            var count = await _notificationRepo.CountAsync(spec, cancellationToken);

            return Result<int>.Success(count);
        }
    }
}