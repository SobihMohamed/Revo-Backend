using Revo.Application.Common.Pagination;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.Application.Features.ContactRequests.Queries.Specifications;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Queries.GetAll
{
    public class GetAllContactRequestsQueryHandler : IQueryHandler<GetAllContactRequestsQuery, PaginationResponse<ContactRequestDto>>
    {
        private readonly IGenericRepo<ContactRequest> _contactRequestRepo;

        public GetAllContactRequestsQueryHandler(IGenericRepo<ContactRequest> contactRequestRepo)
        {
            _contactRequestRepo = contactRequestRepo;
        }

        public async Task<Result<PaginationResponse<ContactRequestDto>>> Handle(GetAllContactRequestsQuery request, CancellationToken cancellationToken)
        {
            var spec = new ContactRequestsWithPaginationSpec(request.SpecParams);
            var countSpec = new ContactRequestsWithPaginationSpec(request.SpecParams);
           
            var contactRequests = await _contactRequestRepo.ListAsync(spec, cancellationToken);
            var totalCount = await _contactRequestRepo.CountAsync(countSpec, cancellationToken);
            
            var response = new PaginationResponse<ContactRequestDto>(
                request.SpecParams.PageIndex,
                request.SpecParams.PageSize,
                totalCount,
                contactRequests
            );

            return Result<PaginationResponse<ContactRequestDto>>.Success(response);
        }
    }
}