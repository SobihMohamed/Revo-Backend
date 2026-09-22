using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Windows.Input;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Commands.Delete
{
    public class DeleteServiceCommandHandler : ICommandHandler<DeleteServiceCommand, bool>
    {
        private readonly IGenericRepo<Service> _serviceRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadService _uploadService;
        public DeleteServiceCommandHandler(IGenericRepo<Service> serviceRepo, IUnitOfWork unitOfWork, IUploadService uploadService)
        {
            _serviceRepo = serviceRepo;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
        }
        public async Task<Result<bool>> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the service entity from the repository
            var service = await _serviceRepo.GetByIdAsync(request.Id,cancellationToken);
            if(service == null)
                return Result<bool>.Failure(new Error("ServiceNotFound", "The service with the specified ID was not found."));
            // Delete From Db First 
            string? publicIdToDelete = service.ImagePublicId;
            try
            {
                await _serviceRepo.DeleteAsync(service, cancellationToken);
                await _unitOfWork.SaveChanges(cancellationToken);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new Error("ServiceDeleteFailed", $"Failed to delete the service: {ex.Message}"));
            }
            if (!string.IsNullOrEmpty(publicIdToDelete))
                await _uploadService.DeleteFileAsync(publicIdToDelete);

            return Result<bool>.Success(true);
        }
    }
}
