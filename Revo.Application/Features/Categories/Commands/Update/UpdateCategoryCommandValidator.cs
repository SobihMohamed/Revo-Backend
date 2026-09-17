using FluentValidation;
using Revo.Application.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty).WithMessage("Category ID is required.");
            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage("Arabic name is required.")
                .MaximumLength(100).WithMessage("Arabic name must not exceed 100 characters.");
            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required.")
                .MaximumLength(100).WithMessage("English name must not exceed 100 characters.");
            When(x => x.ImageUploadDto != null, () =>
            {
                RuleFor(x => x.ImageUploadDto!)
                    .SetValidator(new ImageUploadValidator());
            });
        }
    }
}
