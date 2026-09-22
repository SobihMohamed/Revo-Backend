using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Services.Dto;
using Revo.Application.Features.Services.Specifications;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Queries.GetById
{
    public class GetServiceByIdQueryHandler : IQueryHandler<GetServiceByIdQuery, ServiceDto>
    {
        private readonly IGenericRepo<Service> _serviceRepo;

        public GetServiceByIdQueryHandler(IGenericRepo<Service> serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public async Task<Result<ServiceDto>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new ServiceByIdSpecification(request.Id);

            var serviceDto = await _serviceRepo.FirstOrDefaultAsync(spec, cancellationToken);

            if (serviceDto == null)
                return Result<ServiceDto>.Failure(new Error("Service.NotFound", "The service with the specified ID was not found."));

            return Result<ServiceDto>.Success(serviceDto);
        }
    }
}
