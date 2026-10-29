using FluentValidation.TestHelper;
using Revo.Application.Dto;
using Revo.Application.Features.Services.Commands.Create;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Service.Commands.Create
{
    public class CreateServiceCommandValidatorTests
    {
        private readonly CreateServiceCommandValidator _validator;

        public CreateServiceCommandValidatorTests()
        {
            _validator = new CreateServiceCommandValidator();
        }
        private ImageUploadDto CreateDummyImageDto()
        {
            var dummyStream = new MemoryStream(new byte[] { 1 });
            return new ImageUploadDto(dummyStream, "dummy.jpg");
        }
        [Fact]
        public void Should_Have_Error_When_NameAr_Is_Empty()
        {
            // Arrange

            var command = new CreateServiceCommand("", "NameEn", "DescAr", "DescEn", 1, CreateDummyImageDto()   );

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NameAr)
                  .WithErrorMessage("Arabic name is required.");
        }

        [Fact]
        public void Should_Have_Error_When_NameEn_Exceeds_MaxLength()
        {
            // Arrange
            // 101 characters (maximum is 100)
            var longName = new string('A', 101); 
            var command = new CreateServiceCommand("NameAr", longName, "DescAr", "DescEn", 1, CreateDummyImageDto());

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NameEn)
                  .WithErrorMessage("English name must not exceed 100 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_OrderIndex_Is_Negative()
        {
            // Arrange
            var command = new CreateServiceCommand("NameAr", "NameEn", "DescAr", "DescEn", -1, CreateDummyImageDto());

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.OrderIndex)
                  .WithErrorMessage("Order index must be a non-negative integer.");
        }

        [Fact]
        public void Should_Have_Error_When_Image_Is_Null()
        {
            // Arrange
            var command = new CreateServiceCommand("NameAr", "NameEn", "DescAr", "DescEn", 1, null!);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UploadDto)
                  .WithErrorMessage("Image upload data is required.");
        }
    }
}