using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Dto;
using Revo.Application.Features.Categories.Commands.Create;

namespace Revo.UnitTests.FeatureTest.Category.Command.Create
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Category>> _categoryRepoMock;
        private readonly Mock<IUploadService> _uploadServiceMock;
        private readonly CreateCategoryCommandHandler _handler;
        public CreateCategoryCommandHandlerTests()
        {
            _categoryRepoMock = new Mock<IGenericRepo<Domain.Entities.Category>>();
            _uploadServiceMock = new Mock<IUploadService>();
            _handler = new CreateCategoryCommandHandler(_categoryRepoMock.Object, _uploadServiceMock.Object);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ImageUploadFails()
        {
            // Arrange
            using var fakeStream = new MemoryStream(new byte[] { 1, 2, 3 });
            var imageDto = new ImageUploadDto(fakeStream, "fake-image.png");

            var command = new CreateCategoryCommand(
                "Valid Arabic Name",
                "Valid English Name",
                1,
                imageDto
            );
            _uploadServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UploadResult?)null); // Simulate upload failure
           
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Category.ImageUploadFailed", result.Error.Code);
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_And_CallRepository_When_UploadSucceeds()
        {
            // Arrange
            using var fakeStream = new MemoryStream(new byte[] { 1, 2, 3 });
            var imageDto = new ImageUploadDto(fakeStream, "fake-image.png");
            var command = new CreateCategoryCommand(
                "Valid Arabic Name",
                "Valid English Name",
                1,
                imageDto
            );
            var uploadResult = new UploadResult
            (
                "http://example.com/fake-image.png",
                "fake-public-id"
            );
            _uploadServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResult);
         
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
           
            // Assert
            Assert.True(result.IsSuccess);
            _categoryRepoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
