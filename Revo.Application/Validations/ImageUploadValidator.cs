using FluentValidation;
using Revo.Application.Abstraction.Services;
using Revo.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Validations
{
    public class ImageUploadValidator : AbstractValidator<ImageUploadDto>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long _maxFileSize = 5 * 1024 * 1024;
        public ImageUploadValidator()
        {
            RuleFor(u => u.Content)
                .NotEmpty()
                .WithMessage("Content is required.")
                .Must(stream => stream.Length > 0).WithMessage("File cannot be empty.")
                .Must(stream => stream.Length <= _maxFileSize).WithMessage("File size exceeds the maximum limit of 5MB.");

            RuleFor(x => x.FileName)
                        .NotEmpty().WithMessage("File name is required.")
                        .Must(BeAValidExtension)
                        .WithMessage($"Invalid file extension. Allowed extensions are: {string.Join(", ", _allowedExtensions)}");

        }
        private bool BeAValidExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return _allowedExtensions.Contains(ext);
        }
    }
}
