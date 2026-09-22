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

namespace Revo.Application.Features.Services.Commands.Update
{
    public class UpdateServiceCommandHandler : ICommandHandler<UpdateServiceCommand, Guid>
    {
        private readonly IGenericRepo<Service> _serviceRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadService _uploadService;
        public UpdateServiceCommandHandler(IGenericRepo<Service> serviceRepo, IUnitOfWork unitOfWork, IUploadService uploadService)
        {
            _serviceRepo = serviceRepo;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
        }
        public async Task<Result<Guid>> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            var spec = new ServiceByNameSpecification(request.NameAr, request.NameEn, request.Id);

            var existingService = await _serviceRepo.FirstOrDefaultAsync(spec,cancellationToken);

            if (existingService != null)
            {
                return Result<Guid>.Failure(new Error("Service.DuplicateName", "The service name (in Arabic or English) is already registered."));
            }
            // check if service exists
            var service = await _serviceRepo.GetByIdAsync(request.Id, cancellationToken);
            if(service == null)
                return Result<Guid>.Failure(new Error("Service.NotFound", "Service not found"));
            // update service properties
            service.NameAr = request.NameAr;
            service.NameEn = request.NameEn;
            service.DescriptionAr = request.DescriptionAr;
            service.DescriptionEn = request.DescriptionEn;
            service.OrderIndex = request.OrderIndex;

            string? publicIdToDelete = null;
            string? publicIdToRollback = null;
            // check if upload new image
            if (request.UploadDto != null)
            {
                publicIdToDelete = service.ImagePublicId;
                // upload new image
                var uploadResult = await _uploadService.UploadFileAsync(request.UploadDto.Content,request.UploadDto.FileName, cancellationToken);
                if (uploadResult == null)
                    return Result<Guid>.Failure(new Error("Service.ImageUploadFailed", "Image upload failed"));
                publicIdToRollback = uploadResult.PublicId;
                // update service image properties
                service.ImageUrl = uploadResult.Url;
                service.ImagePublicId = uploadResult.PublicId;
            }
            try
            {
                await _serviceRepo.UpdateAsync(service, cancellationToken);
                await _unitOfWork.SaveChanges(cancellationToken);
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(publicIdToRollback))
                    await _uploadService.DeleteFileAsync(publicIdToRollback);
                return Result<Guid>.Failure(new Error("Service.UpdateFailed", "Service update failed"));
            }

            if (!string.IsNullOrEmpty(publicIdToDelete))
                await _uploadService.DeleteFileAsync(publicIdToDelete);
            return Result<Guid>.Success(service.Id);
        }
    }
}
