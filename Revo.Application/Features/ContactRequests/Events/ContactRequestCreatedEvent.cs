using MediatR;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.ContactRequests.Events
{
    public record ContactRequestCreatedEvent(ContactRequest ContactRequest) : INotification;
}
