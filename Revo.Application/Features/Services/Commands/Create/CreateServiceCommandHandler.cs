using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Services.Specifications;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Commands.Create
{
    public class CreateServiceCommandHandler : ICommandHandler<CreateServiceCommand, Guid>
    {
        private readonly IGenericRepo<Service> _serviceRepo;
        private readonly IUploadService _uploadService;
        private readonly IUnitOfWork _unitOfWork;
        public CreateServiceCommandHandler(IGenericRepo<Service> genericRepo, IUploadService uploadService, IUnitOfWork unitOfWork)
        {
            _serviceRepo = genericRepo;
            _uploadService = uploadService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var spec = new ServiceByNameSpecification(request.NameAr, request.NameEn);

            var existingService = await _serviceRepo.FirstOrDefaultAsync(spec , cancellationToken);

            if (existingService != null)
            {
                return Result<Guid>.Failure(new Error("Service.DuplicateName", "The service name (in Arabic or English) is already registered."));
            }
            // call the upload service to upload the image and get the image URL
            var UploadResult = await _uploadService.UploadFileAsync(
                request.UploadDto.Content,
                request.UploadDto.FileName,
                cancellationToken);
            if (UploadResult == null)
                return Result<Guid>.Failure(new Error("UploadFailed", "Failed to upload the image."));

            // create a new service entity
            var service = new Service
            {
                NameAr = request.NameAr,
                NameEn = request.NameEn,
                DescriptionAr = request.DescriptionAr,
                DescriptionEn = request.DescriptionEn,
                OrderIndex = request.OrderIndex,
                ImageUrl = UploadResult.Url,
                ImagePublicId = UploadResult.PublicId
            };
            await _serviceRepo.AddAsync(service, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
            return Result<Guid>.Success(service.Id);
        }
    }
}
