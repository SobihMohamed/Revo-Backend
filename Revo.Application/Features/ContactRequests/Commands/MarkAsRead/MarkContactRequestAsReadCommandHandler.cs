using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Commands.MarkAsRead
{
    public class MarkContactRequestAsReadCommandHandler : ICommandHandler<MarkContactRequestAsReadCommand, bool>
    {
        private readonly IGenericRepo<ContactRequest> _contactRequestRepo;
        private readonly IUnitOfWork _unitOfWork;


        public MarkContactRequestAsReadCommandHandler(IGenericRepo<ContactRequest> contactRequestRepo, IUnitOfWork unitOfWork)
        {
            _contactRequestRepo = contactRequestRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(MarkContactRequestAsReadCommand request, CancellationToken cancellationToken)
        {
            var contactRequest = await _contactRequestRepo.GetByIdAsync(request.Id, cancellationToken);

            if (contactRequest == null)
            {
                return Result<bool>.Failure(new Error(
                    "ContactRequest.NotFound",
                    $"Contact request with ID {request.Id} not found."));
            }

            if (contactRequest.IsRead)
                return Result<bool>.Success(true);

            contactRequest.IsRead = true;

            await _contactRequestRepo.UpdateAsync(contactRequest, cancellationToken);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}