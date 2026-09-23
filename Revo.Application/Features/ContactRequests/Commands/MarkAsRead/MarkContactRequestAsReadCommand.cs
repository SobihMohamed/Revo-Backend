using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.ContactRequests.Commands.MarkAsRead
{
    public record MarkContactRequestAsReadCommand(Guid Id) : ICommand<bool>;
}
