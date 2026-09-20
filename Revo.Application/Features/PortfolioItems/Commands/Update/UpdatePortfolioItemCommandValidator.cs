using FluentValidation;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Commands.Update
{
    public class UpdatePortfolioItemCommandValidator : AbstractValidator<UpdatePortfolioItemCommand>
    {
        public UpdatePortfolioItemCommandValidator()
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
                .SetValidator(new UpdatePortfolioMediaCommandItemValidator());
        }
    }
    public class UpdatePortfolioMediaCommandItemValidator : AbstractValidator<UpdatePortfolioMediaCommandItem>
    {
        public UpdatePortfolioMediaCommandItemValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type must be a valid MediaType.");

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0).WithMessage("OrderIndex must be greater than or equal to 0.");

            When(x => x.Type == MediaType.Image && x.Id == null, () =>
            {
                RuleFor(x => x.File)
                    .NotNull().WithMessage("Image file is required when adding a new image.");
            });

            When(x => x.Type == MediaType.Video, () =>
            {
                RuleFor(x => x.VideoUrl)
                    .NotEmpty().WithMessage("Video URL is required when Type is Video.")
                    .Must(BeAValidUrl).WithMessage("Video URL must be a valid URL.");
            });
        }
        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
