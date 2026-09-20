using FluentValidation.TestHelper;
using Revo.Application.Features.PortfolioItems.Commands.Update;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Commands.Update
{
    // =================================================================
    // 2. Tests for the Media Item Validator (The Delta Logic)
    // =================================================================
    public class UpdatePortfolioMediaCommandItemValidatorTests
    {
        private readonly UpdatePortfolioMediaCommandItemValidator _validator;

        public UpdatePortfolioMediaCommandItemValidatorTests()
        {
            _validator = new UpdatePortfolioMediaCommandItemValidator();
        }

        [Fact]
        public void Should_Have_Error_When_OrderIndex_Is_Negative()
        {
            var item = new UpdatePortfolioMediaCommandItem(Guid.NewGuid(), MediaType.Image, -1, null, null, null);
            var result = _validator.TestValidate(item);
            result.ShouldHaveValidationErrorFor(x => x.OrderIndex);
        }


        [Fact]
        public void Should_Have_Error_When_NewImage_Has_No_File()
        {
            var item = new UpdatePortfolioMediaCommandItem(null, MediaType.Image, 1, null, null, null);
            var result = _validator.TestValidate(item);
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("Image file is required when adding a new image.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_ExistingImage_Has_No_File()
        {

            var item = new UpdatePortfolioMediaCommandItem(Guid.NewGuid(), MediaType.Image, 1, null, null, null);
            var result = _validator.TestValidate(item);
            result.ShouldNotHaveValidationErrorFor(x => x.File);
        }


        [Fact]
        public void Should_Have_Error_When_Video_Has_No_Url()
        {
            var item = new UpdatePortfolioMediaCommandItem(null, MediaType.Video, 1, "", null, null);
            var result = _validator.TestValidate(item);
            result.ShouldHaveValidationErrorFor(x => x.VideoUrl);
        }

        [Fact]
        public void Should_Have_Error_When_VideoUrl_Is_Invalid()
        {
            var item = new UpdatePortfolioMediaCommandItem(null, MediaType.Video, 1, "not-a-valid-url", null, null);
            var result = _validator.TestValidate(item);
            result.ShouldHaveValidationErrorFor(x => x.VideoUrl)
                  .WithErrorMessage("Video URL must be a valid URL.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_VideoUrl_Is_Valid()
        {
            // فيديو اللينك بتاعه سليم
            var item = new UpdatePortfolioMediaCommandItem(null, MediaType.Video, 1, "https://youtube.com/watch?v=123", null, null);
            var result = _validator.TestValidate(item);
            result.ShouldNotHaveValidationErrorFor(x => x.VideoUrl);
        }
    }
}
