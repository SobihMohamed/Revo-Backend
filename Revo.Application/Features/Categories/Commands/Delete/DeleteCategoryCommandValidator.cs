using FluentValidation;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Commands.Delete
{
    public class DeleteCategoryCommandValidator : AbstractValidator<Category>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(category => category.Id)
                .NotEmpty().WithMessage("Category Id is required.")
                .Must(id => id != Guid.Empty).WithMessage("Category Id cannot be empty.");
        }
    }
}
