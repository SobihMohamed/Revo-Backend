using FluentValidation;
using Revo.Application.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Services.Commands.Update
{
    public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
    {
        public UpdateServiceCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Service ID is required.");

            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage("Arabic name is required.")
                .MaximumLength(100).WithMessage("Arabic name must not exceed 100 characters.");

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required.")
                .MaximumLength(100).WithMessage("English name must not exceed 100 characters.");

            RuleFor(x => x.DescriptionAr)
                .NotEmpty().WithMessage("Arabic description is required.")
                .MaximumLength(500).WithMessage("Arabic description must not exceed 500 characters.");

            RuleFor(x => x.DescriptionEn)
                .NotEmpty().WithMessage("English description is required.")
                .MaximumLength(500).WithMessage("English description must not exceed 500 characters.");

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0).WithMessage("Order index must be a non-negative integer.");

            When(x => x.UploadDto != null, () =>
            {
                RuleFor(x => x.UploadDto!)
                    .SetValidator(new ImageUploadValidator());
            });
        }
    }
}