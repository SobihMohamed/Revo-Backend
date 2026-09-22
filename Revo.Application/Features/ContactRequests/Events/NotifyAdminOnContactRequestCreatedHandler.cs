using MediatR;
using Revo.Application.Common.Notifications;
using Revo.Application.Contracts.Notifications;
using Revo.Application.Contracts.Repositories;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.ContactRequests.Events
{
    public class NotifyAdminOnContactRequestCreatedHandler : INotificationHandler<ContactRequestCreatedEvent>
    {
        private readonly IGenericRepo<Notification> _notificationRepo;
        private readonly INotificationDispatcher _dispatcher;

        public NotifyAdminOnContactRequestCreatedHandler(
            IGenericRepo<Notification> notificationRepo,
            INotificationDispatcher dispatcher)
        {
            _notificationRepo = notificationRepo;
            _dispatcher = dispatcher;
        }

        public async Task Handle(ContactRequestCreatedEvent notificationEvent, CancellationToken cancellationToken)
        {
            var request = notificationEvent.ContactRequest;

            var dbNotification = new Notification
            {
                TitleAr = "طلب تواصل جديد",
                TitleEn = "New Contact Request",
                MessageAr = $"يوجد طلب تواصل جديد من {request.Name}",
                MessageEn = $"There is a new contact request from {request.Name}",
                TargetUrl = $"/admin/contact-requests/{request.Id}",
                IsRead = false
            };

            await _notificationRepo.AddAsync(dbNotification, cancellationToken);

            var notificationMessage = new NotificationMessage
            {
                NotificationId = dbNotification.Id,
                TitleAr = dbNotification.TitleAr,
                TitleEn = dbNotification.TitleEn,
                MessageAr = dbNotification.MessageAr,
                MessageEn = dbNotification.MessageEn,
                TargetUrl = dbNotification.TargetUrl,
                // whatsApp inside the strategy performed
                Channels = new List<NotificationChannel>
                {
                    NotificationChannel.Push,
                    NotificationChannel.WhatsApp
                }
            };

            await _dispatcher.DispatchAsync(notificationMessage, cancellationToken);
        }
    }
}