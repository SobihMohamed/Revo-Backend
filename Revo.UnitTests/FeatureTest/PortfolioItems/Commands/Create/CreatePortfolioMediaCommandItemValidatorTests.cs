using FluentValidation.TestHelper;
using Revo.Application.Dto;
using Revo.Application.Features.PortfolioItems.Commands.Create;
namespace Revo.UnitTests.FeatureTest.PortfolioItems.Commands.Create
{
    public class CreatePortfolioMediaCommandItemValidatorTests
    {
        private readonly CreatePortfolioMediaCommandItemValidator _validator;
        public CreatePortfolioMediaCommandItemValidatorTests()
        {
            _validator = new CreatePortfolioMediaCommandItemValidator();
        }
        [Fact]
        public void Should_Have_Error_When_Type_Is_Invalid()
        {
            var model = new CreatePortfolioMediaCommandItem((Domain.Enums.MediaType)999, 1, null, null, null);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Type);
        }

        [Fact]
        public void Should_Have_Error_When_OrderIndex_Is_Negative()
        {
            var model = new CreatePortfolioMediaCommandItem(Domain.Enums.MediaType.Image, -1, null, null, null);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OrderIndex);
        }

        [Fact]
        public void Should_Have_Error_When_Image_Has_No_File()
        {
            var model = new CreatePortfolioMediaCommandItem(Domain.Enums.MediaType.Image, 1, null, null, null);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("Image file is required when MediaType is Image.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Image_Has_File()
        {
            var dummyFile = new ImageUploadDto(new MemoryStream(), "test.png");
            var model = new CreatePortfolioMediaCommandItem(Domain.Enums.MediaType.Image, 1, dummyFile, null, null);

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.File);
        }

        [Fact]
        public void Should_Have_Error_When_Video_Has_No_Url()
        {
            var model = new CreatePortfolioMediaCommandItem(Domain.Enums.MediaType.Video, 1, null, "", null);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.VideoUrl)
                  .WithErrorMessage("Video URL is required when MediaType is Video.");
        }

        [Fact]
        public void Should_Have_Error_When_Video_Has_Invalid_Url()
        {
            var model = new CreatePortfolioMediaCommandItem(Domain.Enums.MediaType.Video, 1, null, "not-a-valid-url", null);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.VideoUrl)
                  .WithErrorMessage("Video URL must be a valid link.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Video_Has_Valid_Url()
        {
            var model = new CreatePortfolioMediaCommandItem(Domain.Enums.MediaType.Video, 1, null, "https://vimeo.com/123456", null);
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.VideoUrl);
        }
    }
}
