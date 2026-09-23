using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.ContactRequests.Commands.Create
{
    public class CreateContactRequestCommandValidator : AbstractValidator<CreateContactRequestCommand>
    {
        public CreateContactRequestCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{11}$").WithMessage("Phone number must be 11 digits.");
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters.");

        }
    }
}
