using MediatR;
using Revo.Application.Common.Notifications;
using Revo.Application.Contracts.Notifications;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Services.Specifications;
using Revo.Domain.Entities;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Revo.Application.Features.ContactRequests.Events
{
    public class NotifyAdminOnContactRequestCreatedHandler : INotificationHandler<ContactRequestCreatedEvent>
    {
        private readonly IGenericRepo<Notification> _notificationRepo;
        private readonly IGenericRepo<Service> _serviceRepo;
        private readonly INotificationDispatcher _dispatcher;

        public NotifyAdminOnContactRequestCreatedHandler(
            IGenericRepo<Notification> notificationRepo,
            IGenericRepo<Service> serviceRepo,
            INotificationDispatcher dispatcher)
        {
            _notificationRepo = notificationRepo;
            _serviceRepo = serviceRepo;
            _dispatcher = dispatcher;
        }

        public async Task Handle(ContactRequestCreatedEvent notificationEvent, CancellationToken cancellationToken)
        {
            var request = notificationEvent.ContactRequest;

            string serviceName = "خدمة غير محددة";

            if (request.ServiceId.HasValue)
            {
                var serviceDto = await _serviceRepo.FirstOrDefaultAsync(new ServiceByIdSpecification(request.ServiceId.Value), cancellationToken);
                serviceName = serviceDto?.NameAr ?? "خدمة غير محددة";
            }

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

            var pushNotificationMessage = new NotificationMessage
            {
                NotificationId = dbNotification.Id,
                TitleAr = dbNotification.TitleAr,
                TitleEn = dbNotification.TitleEn,
                MessageAr = dbNotification.MessageAr,
                MessageEn = dbNotification.MessageEn,
                TargetUrl = dbNotification.TargetUrl,
                Channels = new List<NotificationChannel> { NotificationChannel.Push }
            };

            await _dispatcher.DispatchAsync(pushNotificationMessage, cancellationToken);

            var twilioVariables = new Dictionary<string, string>
            {
                { "1", SanitizeForWhatsApp(request.Name, "عميل") },
                { "2", SanitizeForWhatsApp(request.PhoneNumber, "غير متوفر") },
                { "3", SanitizeForWhatsApp(serviceName, "خدمة غير محددة") },
                { "4", SanitizeForWhatsApp(request.Message, "لا توجد رسالة") }
            };

            var whatsAppNotificationMessage = new NotificationMessage
            {
                MessageAr = JsonSerializer.Serialize(twilioVariables),
                Channels = new List<NotificationChannel> { NotificationChannel.WhatsApp }
            };

            await _dispatcher.DispatchAsync(whatsAppNotificationMessage, cancellationToken);
        }

        private static string SanitizeForWhatsApp(string? input, string fallback)
        {
            if (string.IsNullOrWhiteSpace(input))
                return fallback;

            var cleaned = input.Replace("\r\n", " - ")
                               .Replace("\n", " - ")
                               .Replace("\r", " - ")
                               .Replace("\t", " ");

            return Regex.Replace(cleaned, @"\s{2,}", " ").Trim();
        }
    }
}