using Ardalis.Specification;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Queries.Specifications
{
    public class ContactRequestByIdSpec : SingleResultSpecification<ContactRequest, ContactRequestDto>
    {
        public ContactRequestByIdSpec(Guid id)
        {
            Query.Where(x => x.Id == id);

            Query.Select(x => new ContactRequestDto
            {
                Id = x.Id,
                Name = x.Name,
                PhoneNumber = x.PhoneNumber,
                Message = x.Message!,
                IsRead = x.IsRead,
                ServiceId = x.ServiceId,
                CreatedAt = x.CreatedAt
            });
        }
    }
}
