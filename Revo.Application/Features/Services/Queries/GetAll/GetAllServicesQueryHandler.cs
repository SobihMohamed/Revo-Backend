using Revo.Application.Common.Pagination;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Services.Dto;
using Revo.Application.Features.Services.Specifications;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Queries.GetAll
{
    public class GetAllServicesQueryHandler : IQueryHandler<GetAllServicesQuery, PaginationResponse<ServiceDto>>
    {
        private readonly IGenericRepo<Service> _serviceRepo;

        public GetAllServicesQueryHandler(IGenericRepo<Service> serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public async Task<Result<PaginationResponse<ServiceDto>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            var countSpec = new ServicesCountSpecification(request.SpecParams);
            var totalItems = await _serviceRepo.CountAsync(countSpec, cancellationToken);

            var dataSpec = new ServicesWithPaginationSpecification(request.SpecParams);

            var services = await _serviceRepo.ListAsync(dataSpec, cancellationToken);

            var response = new PaginationResponse<ServiceDto>(
                request.SpecParams.PageIndex,
                request.SpecParams.PageSize,
                totalItems,
                services
            );

            return Result<PaginationResponse<ServiceDto>>.Success(response);
        }
    }
}