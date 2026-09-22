using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Commands
{
    public record CreateContactRequestCommand(
        string Name,
        string PhoneNumber,
        string Message, 
        Guid? ServiceId
    ) : ICommand<Guid>;
}
