using FluentValidation;
using Revo.Application.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(c => c.NameAr)
                .NotEmpty().WithMessage("Arabic name is required.")
                .MaximumLength(100).WithMessage("Arabic name must not exceed 100 characters.");

            RuleFor(c => c.NameEn)
                .NotEmpty().WithMessage("English name is required.")
                .MaximumLength(100).WithMessage("English name must not exceed 100 characters.");

            RuleFor(c => c.OrderIndex)
                .GreaterThanOrEqualTo(0).WithMessage("Order index must be a non-negative integer.");    
            
            RuleFor(c => c.ImageUploadDto)
                .NotNull().WithMessage("Image upload information is required.")
                .SetValidator(new ImageUploadValidator());
        }
    }
}
