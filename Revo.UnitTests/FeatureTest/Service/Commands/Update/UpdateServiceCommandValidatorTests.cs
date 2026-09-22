using FluentValidation.TestHelper;
using Revo.Application.Dto;
using Revo.Application.Features.Services.Commands.Update;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Service.Commands.Update
{
    public class UpdateServiceCommandValidatorTests
    {
        private readonly UpdateServiceCommandValidator _validator;

        public UpdateServiceCommandValidatorTests()
        {
            _validator = new UpdateServiceCommandValidator();
        }

        private ImageUploadDto CreateDummyImageDto()
        {
            var dummyStream = new MemoryStream(new byte[] { 1 });
            return new ImageUploadDto(dummyStream, "dummy.jpg");
        }
        [Fact]
        public void Should_Have_NoError_When_Command_Is_Valid()
        {
            // Arrange
            var command = new UpdateServiceCommand(Guid.NewGuid(), "NameAr", "NameEn", "DescAr", "DescEn", 1, CreateDummyImageDto());
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            // Arrange
            var command = new UpdateServiceCommand(Guid.Empty, "NameAr", "NameEn", "DescAr", "DescEn", 1, null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("Service ID is required.");
        }

        [Fact]
        public void Should_Have_Error_When_NameAr_Is_Empty()
        {
            // Arrange
            var command = new UpdateServiceCommand(Guid.NewGuid(), "", "NameEn", "DescAr", "DescEn", 1, null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NameAr)
                  .WithErrorMessage("Arabic name is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_NewImage_Is_Null()
        {
            // Arrange
            var command = new UpdateServiceCommand(Guid.NewGuid(), "NameAr", "NameEn", "DescAr", "DescEn", 1, null);

            // Act
            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.UploadDto);
        }
    }
}