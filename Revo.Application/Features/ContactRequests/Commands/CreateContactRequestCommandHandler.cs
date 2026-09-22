using MediatR;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.ContactRequests.Events;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Commands
{
    public class CreateContactRequestCommandHandler : ICommandHandler<CreateContactRequestCommand, Guid>
    {
        private readonly IGenericRepo<ContactRequest> _contactRequestRepo;
        private readonly IPublisher _publisher;
        public CreateContactRequestCommandHandler(
            IGenericRepo<ContactRequest> contactRequestRepo,
            IPublisher publisher)
        {
            _contactRequestRepo = contactRequestRepo;
            _publisher = publisher;
        }
        public async Task<Result<Guid>> Handle(CreateContactRequestCommand request, CancellationToken cancellationToken)
        {
            // 1. Map to Entity
            var contactRequest = new ContactRequest
            {
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Message = request.Message,
                ServiceId = request.ServiceId,
                 IsRead = false 
            };

            // 2. Save to Database
            await _contactRequestRepo.AddAsync(contactRequest, cancellationToken);
            // 3. Publish Event
            await _publisher.Publish(new ContactRequestCreatedEvent(contactRequest), cancellationToken);

            // 4. Return Success
            return Result<Guid>.Success(contactRequest.Id);
        }
    }
}
