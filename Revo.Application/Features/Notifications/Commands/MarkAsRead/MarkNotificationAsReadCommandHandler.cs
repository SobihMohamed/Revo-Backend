using Revo.Application.Contracts.Repositories;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkNotificationAsReadCommandHandler : ICommandHandler<MarkNotificationAsReadCommand, bool>
    {
        private readonly IGenericRepo<Notification> _notificationRepo;

        public MarkNotificationAsReadCommandHandler(IGenericRepo<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<Result<bool>> Handle(
            MarkNotificationAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var notification = await _notificationRepo.GetByIdAsync(request.Id, cancellationToken);

            if (notification == null)
                return Result<bool>.Failure(new Error("Notification.NotFound", "الإشعار غير موجود"));

            if (notification.IsRead)
                return Result<bool>.Success(true);

            notification.IsRead = true;
            await _notificationRepo.UpdateAsync(notification, cancellationToken);


            return Result<bool>.Success(true);
        }
    }
}