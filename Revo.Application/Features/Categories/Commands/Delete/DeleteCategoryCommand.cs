using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Commands.Delete
{
    public record DeleteCategoryCommand(Guid Id) : ICommand<bool>;
}
