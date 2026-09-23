using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.Application.Features.ContactRequests.Queries.Specifications;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Queries.GetById
{
    public class GetContactRequestByIdQueryHandler : IQueryHandler<GetContactRequestByIdQuery, ContactRequestDto>
    {
        private readonly IGenericRepo<ContactRequest> _contactRequestRepo;

        public GetContactRequestByIdQueryHandler(IGenericRepo<ContactRequest> contactRequestRepo)
        {
            _contactRequestRepo = contactRequestRepo;
        }

        public async Task<Result<ContactRequestDto>> Handle(GetContactRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new ContactRequestByIdSpec(request.Id);

            var contactRequestDto = await _contactRequestRepo.FirstOrDefaultAsync(spec, cancellationToken);

            if (contactRequestDto == null)
            {
                return Result<ContactRequestDto>.Failure(new Error(
                    "ContactRequest.NotFound",
                    $"Contact request with ID {request.Id} not found."));
            }

            return Result<ContactRequestDto>.Success(contactRequestDto);
        }
    }
}