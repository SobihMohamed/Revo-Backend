using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Commands.Delete
{
    public record DeleteServiceCommand(Guid Id) : ICommand<bool>;
}
