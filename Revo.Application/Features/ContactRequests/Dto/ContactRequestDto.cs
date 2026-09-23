using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.ContactRequests.Dto
{
    public class ContactRequestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public Guid? ServiceId { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
