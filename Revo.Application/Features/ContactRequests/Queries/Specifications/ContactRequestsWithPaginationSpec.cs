using Ardalis.Specification;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.Application.Features.ContactRequests.Queries.Helper;
using Revo.Domain.Entities;
using System;

namespace Revo.Application.Features.ContactRequests.Queries.Specifications
{
    public class ContactRequestsWithPaginationSpec : Specification<ContactRequest, ContactRequestDto>
    {
        public ContactRequestsWithPaginationSpec(ContactRequestSpecParams contactRequestParams)
        {
            if (!string.IsNullOrEmpty(contactRequestParams.Search))
            {
                var search = contactRequestParams.Search.ToLower();
                Query.Where(x => x.Name.ToLower().Contains(search) ||
                                 x.PhoneNumber.ToLower().Contains(search));
            }

            if (contactRequestParams.IsRead.HasValue)
            {
                Query.Where(x => x.IsRead == contactRequestParams.IsRead.Value);
            }

            if (contactRequestParams.Sort.HasValue)
            {
                switch (contactRequestParams.Sort)
                {
                    case ContactRequestSortOptions.OldestFirst:
                        Query.OrderBy(x => x.CreatedAt);
                        break;
                    case ContactRequestSortOptions.NewestFirst:
                    default:
                        Query.OrderByDescending(x => x.CreatedAt);
                        break;
                }
            }
            else
            {
                Query.OrderByDescending(x => x.CreatedAt);
            }

            Query.Select(x => new ContactRequestDto
            {
                Id = x.Id,
                Name = x.Name,
                PhoneNumber = x.PhoneNumber,
                Message = x.Message,
                IsRead = x.IsRead,
                ServiceId = x.ServiceId,
                CreatedAt = x.CreatedAt
            });

            Query.Skip(contactRequestParams.PageSize * (contactRequestParams.PageIndex - 1))
                 .Take(contactRequestParams.PageSize);
        }
    }
}