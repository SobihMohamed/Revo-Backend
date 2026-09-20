using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Commands.Delete
{
    public record DeletePortfolioItemCommand(Guid Id) : ICommand<bool>;
}
