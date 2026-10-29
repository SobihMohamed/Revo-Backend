using Revo.Application.Common.Pagination;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.Application.Features.ContactRequests.Queries.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Queries.GetAll
{
    public record GetAllContactRequestsQuery(ContactRequestSpecParams SpecParams) : IQuery<PaginationResponse<ContactRequestDto>>;
}
