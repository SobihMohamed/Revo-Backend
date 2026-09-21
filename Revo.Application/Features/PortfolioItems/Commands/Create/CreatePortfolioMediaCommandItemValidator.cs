using FluentValidation;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Commands.Create
{
    public class CreatePortfolioMediaCommandItemValidator : AbstractValidator<CreatePortfolioMediaCommandItem>
    {
        public CreatePortfolioMediaCommandItemValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid Media Type.");

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0).WithMessage("Order index cannot be negative.");

            When(x => x.Type == MediaType.Image, () =>
            {
                RuleFor(x => x.File)
                    .NotNull().WithMessage("Image file is required when MediaType is Image.");
            });

            When(x => x.Type == MediaType.Video, () =>
            {
                RuleFor(x => x.VideoUrl)
                    .NotEmpty().WithMessage("Video URL is required when MediaType is Video.")
                    .Must(BeAValidUrl).WithMessage("Video URL must be a valid link.");

            });
        }

        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
        public class CreatePortfolioItemCommandValidator : AbstractValidator<CreatePortfolioItemCommand>
        {
            public CreatePortfolioItemCommandValidator()
            {
                RuleFor(x => x.CaptionAr)
                    .NotEmpty().WithMessage("Arabic caption is required.")
                    .MaximumLength(200);

                RuleFor(x => x.CaptionEn)
                    .NotEmpty().WithMessage("English caption is required.")
                    .MaximumLength(200);

                RuleFor(x => x.CategoryId)
                    .NotEmpty().WithMessage("Category is required.");

                RuleFor(x => x.MediaItems)
                    .NotEmpty().WithMessage("At least one media item is required.")
                    .Must(items => items != null && items.Count > 0).WithMessage("Media items list cannot be empty.");

                RuleForEach(x => x.MediaItems)
                    .SetValidator(new CreatePortfolioMediaCommandItemValidator());
            }
        }
    }
}
