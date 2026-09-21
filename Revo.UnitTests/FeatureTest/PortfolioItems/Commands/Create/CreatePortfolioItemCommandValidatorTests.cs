using FluentValidation.TestHelper;
using Revo.Application.Dto;
using Revo.Application.Features.PortfolioItems.Commands.Create;
using Revo.Domain.Enums;
using static Revo.Application.Features.PortfolioItems.Commands.Create.CreatePortfolioMediaCommandItemValidator;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Commands.Create
{
    public class CreatePortfolioItemCommandValidatorTests
    {
        private readonly CreatePortfolioItemCommandValidator _validator;

        public CreatePortfolioItemCommandValidatorTests()
        {
            _validator = new CreatePortfolioItemCommandValidator();
        }
        [Fact]
        public void Should_Have_Error_When_Captions_Are_Empty()
        {
            var model = new CreatePortfolioItemCommand("", "", 1, Guid.NewGuid(), new List<CreatePortfolioMediaCommandItem>());

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.CaptionAr);
            result.ShouldHaveValidationErrorFor(x => x.CaptionEn);
        }
        [Fact]
        public void Should_Have_Error_When_CategoryId_Is_Empty()
        {
            var model = new CreatePortfolioItemCommand("عربي", "English", 1, Guid.Empty, new List<CreatePortfolioMediaCommandItem>());
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }

        [Fact]
        public void Should_Have_Error_When_MediaItems_List_Is_Empty()
        {
            var model = new CreatePortfolioItemCommand("عربي", "English", 1, Guid.NewGuid(), new List<CreatePortfolioMediaCommandItem>());

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.MediaItems)
                  .WithErrorMessage("At least one media item is required.");
        }

        [Fact]
        public void Should_Not_Have_Any_Errors_When_Command_Is_Valid()
        {
            var validMediaItem = new CreatePortfolioMediaCommandItem(
                MediaType.Image,
                1,
                new ImageUploadDto(new MemoryStream(), "test.png"),
                null,
                null);

            var model = new CreatePortfolioItemCommand(
                "تجربة عربي",
                "Test English",
                1,
                Guid.NewGuid(),
                new List<CreatePortfolioMediaCommandItem> { validMediaItem });

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
