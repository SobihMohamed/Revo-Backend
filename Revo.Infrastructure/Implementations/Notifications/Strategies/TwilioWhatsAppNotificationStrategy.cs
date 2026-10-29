using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Revo.Application.Common.Notifications;
using Revo.Application.Contracts.Notifications;
using Revo.Infrastructure.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Revo.Infrastructure.Implementations.Notifications.Strategies
{
    public class TwilioWhatsAppNotificationStrategy : INotificationStrategy
    {
        private readonly TwilioSettings _twilioSettings;
        private readonly ILogger<TwilioWhatsAppNotificationStrategy> _logger;

        public TwilioWhatsAppNotificationStrategy(
            IOptions<TwilioSettings> twilioSettings,
            ILogger<TwilioWhatsAppNotificationStrategy> logger)
        {
            _twilioSettings = twilioSettings.Value;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.WhatsApp;

        public async Task SendAsync(NotificationMessage message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_twilioSettings.AccountSid) ||
                string.IsNullOrWhiteSpace(_twilioSettings.AuthToken) ||
                string.IsNullOrWhiteSpace(_twilioSettings.TemplateSid) ||
                string.IsNullOrWhiteSpace(_twilioSettings.MessagingServiceSid))
            {
                _logger.LogWarning("Twilio settings (including TemplateSid and MessagingServiceSid) are not configured properly.");
                return;
            }

            try
            {
                TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

                var to = !string.IsNullOrWhiteSpace(message.WhatsAppRecipient)
                            ? $"whatsapp:{message.WhatsAppRecipient}"
                            : _twilioSettings.AdminPhoneNumber;

                var from = _twilioSettings.WhatsAppNumber;

                var messageResource = await MessageResource.CreateAsync(
                    to: new PhoneNumber(to),
                    messagingServiceSid: _twilioSettings.MessagingServiceSid,
                    contentSid: _twilioSettings.TemplateSid,
                    contentVariables: message.MessageAr
                );

                _logger.LogInformation("WhatsApp template message sent successfully. SID: {Sid}", messageResource.Sid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send WhatsApp template message via Twilio.");
            }
        }
    }
}