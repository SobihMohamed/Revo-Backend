using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Services.Commands.Delete
{
    public class DeleteServiceCommandValidator : AbstractValidator<DeleteServiceCommand>
    {
        public DeleteServiceCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Service Id is required.");
        }
    }
}
