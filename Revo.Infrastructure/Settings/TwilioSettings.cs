using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Settings
{
    public class TwilioSettings
    {
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string WhatsAppNumber { get; set; } = string.Empty;
        public string AdminPhoneNumber { get; set; } = string.Empty;
    }
}
