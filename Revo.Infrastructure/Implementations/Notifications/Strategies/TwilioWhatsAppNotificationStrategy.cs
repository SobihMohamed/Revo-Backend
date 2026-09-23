using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Revo.Application.Common.Notifications;
using Revo.Application.Contracts.Notifications;
using Revo.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;
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
                string.IsNullOrWhiteSpace(_twilioSettings.AuthToken))
            {
                _logger.LogWarning("Twilio settings are not configured. WhatsApp message was ignored.");
                return;
            }

            try
            {
                TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

                var to = !string.IsNullOrWhiteSpace(message.WhatsAppRecipient)
                            ? $"whatsapp:{message.WhatsAppRecipient}"
                            : _twilioSettings.AdminPhoneNumber;

                var from = _twilioSettings.WhatsAppNumber;

                var messageBody = $"{message.MessageAr ?? message.MessageEn}";

                var messageResource = await MessageResource.CreateAsync(
                    body: messageBody,
                    from: new PhoneNumber(from),
                    to: new PhoneNumber(to)
                );

                _logger.LogInformation("WhatsApp message sent successfully. SID: {Sid}", messageResource.Sid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send WhatsApp message via Twilio.");
            }
        }
    }
}