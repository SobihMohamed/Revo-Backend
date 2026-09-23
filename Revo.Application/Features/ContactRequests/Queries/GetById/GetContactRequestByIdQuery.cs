using Revo.Application.Features.ContactRequests.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Queries.GetById
{
    public record GetContactRequestByIdQuery(Guid Id) : IQuery<ContactRequestDto>;
}
