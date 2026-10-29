using FluentValidation.TestHelper;
using Revo.Application.Dto;
using Revo.Application.Features.PortfolioItems.Commands.Update;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Commands.Update
{
    // =================================================================
    // 1. Tests for the Main Command Validator
    // =================================================================
    public class UpdatePortfolioItemCommandValidatorTests
    {
        private readonly UpdatePortfolioItemCommandValidator _validator;

        public UpdatePortfolioItemCommandValidatorTests()
        {
            _validator = new UpdatePortfolioItemCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_CaptionAr_Is_Empty()
        {
            var command = new UpdatePortfolioItemCommand(Guid.NewGuid(), "", "En", 1, Guid.NewGuid(), new List<UpdatePortfolioMediaCommandItem> { CreateValidMediaItem() });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CaptionAr);
        }

        [Fact]
        public void Should_Have_Error_When_CaptionEn_Exceeds_MaxLength()
        {
            var command = new UpdatePortfolioItemCommand(Guid.NewGuid(), "Ar", new string('a', 201), 1, Guid.NewGuid(), new List<UpdatePortfolioMediaCommandItem> { CreateValidMediaItem() });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CaptionEn);
        }

        [Fact]
        public void Should_Have_Error_When_MediaItems_Is_Empty()
        {
            var command = new UpdatePortfolioItemCommand(Guid.NewGuid(), "Ar", "En", 1, Guid.NewGuid(), new List<UpdatePortfolioMediaCommandItem>());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MediaItems);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new UpdatePortfolioItemCommand(Guid.NewGuid(), "Caption Ar", "Caption En", 1, Guid.NewGuid(), new List<UpdatePortfolioMediaCommandItem> { CreateValidMediaItem() });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        private UpdatePortfolioMediaCommandItem CreateValidMediaItem()
        {
            return new UpdatePortfolioMediaCommandItem(null, MediaType.Image, 1, null, new ImageUploadDto(new MemoryStream(), "test.png"), null);
        }
    }
}