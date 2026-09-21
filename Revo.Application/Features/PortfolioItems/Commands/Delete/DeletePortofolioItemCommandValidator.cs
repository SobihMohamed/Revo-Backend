using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Commands.Delete
{
    public class DeletePortofolioItemCommandValidator : AbstractValidator<DeletePortfolioItemCommand>
    {
        public DeletePortofolioItemCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Portfolio item Id is required.");
        }
    }
}
