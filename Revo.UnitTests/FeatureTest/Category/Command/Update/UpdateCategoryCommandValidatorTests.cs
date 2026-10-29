using FluentValidation.TestHelper;
using Revo.Application.Dto;
using Revo.Application.Features.Categories.Commands.Update;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Category.Command.Update
{
    public class UpdateCategoryCommandValidatorTests
    {
        private readonly UpdateCategoryCommandValidator _validator;
        public UpdateCategoryCommandValidatorTests()
        {
            _validator = new UpdateCategoryCommandValidator();
        }
        [Fact]
        public void Should_HaveError_When_CategoryIdIsEmpty()
        {
            // Arrange
            var command = new UpdateCategoryCommand(
                Guid.Empty, 
                "Valid Arabic Name",
                "Valid English Name",
                1,
                null
            );
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
        [Fact]
        public void Should_NotHaveError_When_CommandIsValid_And_NoImageProvided()
        {
            // Arrange
            var command = new UpdateCategoryCommand(
                Guid.NewGuid(), 
                "Valid Arabic Name",
                "Valid English Name",
                1,
                null
            );

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_HaveError_When_NameAr_IsEmpty()
        {
            // Arrange
            var command = new UpdateCategoryCommand(
                Guid.NewGuid(),
                "",
                "Valid English Name",
                1,
                null
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
            var command = new UpdateCategoryCommand(
                Guid.NewGuid(),
                "Valid Arabic Name",
                new string('A', 101), // 101 characters
                1,
                null
            );
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NameEn);
        }
        [Fact]
        public void Should_NotHaveError_When_CommandIsValid_And_ImageProvided()
        {
            // Arrange
            using var stream = new MemoryStream(new byte[] { 0x01 });
            var imageDto = new ImageUploadDto(stream, "image.png");
            var command = new UpdateCategoryCommand(
                Guid.NewGuid(),
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
