using FluentValidation.TestHelper;
using Revo.Application.Dto;
using Revo.Application.Features.Categories.Commands.Create;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Category.Command.Create
{
    public class CreateCategoryCommandValidatorTests
    {
        private readonly CreateCategoryCommandValidator _validator;
        public CreateCategoryCommandValidatorTests()
        {
            _validator = new CreateCategoryCommandValidator();
        }

        [Fact]
        public void Should_HaveError_When_NameAr_IsEmpty()
        {
            // Arrange
            using var stream = new MemoryStream(new byte[] { 0x01 });

            var imageDto = new ImageUploadDto(stream, "image.png");

            var command = new CreateCategoryCommand(
                "",
                "Valid English Name",
                1,
                imageDto
            );

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NameAr);
        }
        [Fact]
        public void Should_HaveError_When_NameEn_ExceedsMaxLength()
        {
            // Arrange
            using var stream = new MemoryStream(new byte[] { 0x01 });
            var imageDto = new ImageUploadDto(stream, "image.png");
            var command = new CreateCategoryCommand(
                "Valid Arabic Name",
                new string('A', 101), // 101 characters
                1,
                imageDto
            );
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NameEn);
        }
        [Fact]
        public void Should_HaveError_When_Image_IsNull()
        {
            // Arrange
            var command = new CreateCategoryCommand(
            "Valid Arabic Name",
                "Valid English Name",
                1,
                null // Image is null
            );
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ImageUploadDto);
        }
        [Fact]
        public void Should_NotHaveError_When_CommandIsValid()
        {
            // Arrange
            using var stream = new MemoryStream(new byte[] { 0x01 });
            var imageDto = new ImageUploadDto(stream, "image.png");
            var command = new CreateCategoryCommand(
                "Valid Arabic Name",
                "Valid English Name",
                1,
                imageDto
            );
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
