using Ardalis.Specification;
using Revo.Application.Features.ContactRequests.Queries.Helper;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.ContactRequests.Queries.Specifications
{
    public class ContactRequestsCountSpecification : Specification<ContactRequest>
    {
        public ContactRequestsCountSpecification(ContactRequestSpecParams contactRequestParams)
        {
            if (!string.IsNullOrEmpty(contactRequestParams.Search))
            {
                var search = contactRequestParams.Search.ToLower();
                Query.Where(x => x.Name.ToLower().Contains(search) ||
                                 x.PhoneNumber.ToLower().Contains(search));
            }
            if (contactRequestParams.IsRead.HasValue)
                Query.Where(x => x.IsRead == contactRequestParams.IsRead.Value);
        }
    }
}
