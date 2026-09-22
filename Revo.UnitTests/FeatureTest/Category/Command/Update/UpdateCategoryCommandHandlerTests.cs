using FluentAssertions;
using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Dto;
using Revo.Application.Features.Categories.Commands.Update;
using Revo.Application.Features.Categories.Specifications; 


namespace Revo.UnitTests.FeatureTest.Category.Command.Update
{
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Category>> _categoryRepoMock;
        private readonly UpdateCategoryCommandHandler _handler;
        private readonly Mock<IUploadService> _uploadServiceMock;

        public UpdateCategoryCommandHandlerTests()
        {
            _categoryRepoMock = new Mock<IGenericRepo<Domain.Entities.Category>>();
            _uploadServiceMock = new Mock<IUploadService>();
            _handler = new UpdateCategoryCommandHandler(_categoryRepoMock.Object, _uploadServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CategoryNameIsDuplicate()
        {
            // Arrange
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "Valid Arabic", "Valid English", 1, null);

            _categoryRepoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<CategoryByNameSpecification>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(new Domain.Entities.Category());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Category.DuplicateName", result.Error.Code);

            _categoryRepoMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _categoryRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
        {
            // Arrange
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "Valid Arabic", "Valid English", 1, null);

            _categoryRepoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<CategoryByNameSpecification>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync((Domain.Entities.Category?)null);

            _categoryRepoMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                             .ReturnsAsync((Domain.Entities.Category?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Category.NotFound", result.Error.Code);

            _categoryRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_NoImageProvided()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var command = new UpdateCategoryCommand(categoryId, "Valid Arabic", "Valid English", 1, null);

            var existingCategory = new Domain.Entities.Category
            {
                Id = categoryId,
                NameAr = "Old Arabic",
                NameEn = "Old English",
                OrderIndex = 0,
                ImageUrl = "old_image_url"
            };

            _categoryRepoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<CategoryByNameSpecification>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync((Domain.Entities.Category?)null);

            _categoryRepoMock.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                             .ReturnsAsync(existingCategory);

            // Act 
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(categoryId);

            _categoryRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()), Times.Once);

            _uploadServiceMock.Verify(upload => upload.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_NewImageUploadFails() 
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            using var fakeStream = new MemoryStream(new byte[] { 0x01 });
            var command = new UpdateCategoryCommand(categoryId, "Valid Arabic", "Valid English", 1, new ImageUploadDto(fakeStream, "new_image.png"));

            var existingCategory = new Domain.Entities.Category
            {
                Id = categoryId,
                NameAr = "Old Arabic",
                NameEn = "Old English",
                OrderIndex = 0,
                ImageUrl = "old_image_url"
            };

            _categoryRepoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<CategoryByNameSpecification>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync((Domain.Entities.Category?)null);

            _categoryRepoMock.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                             .ReturnsAsync(existingCategory);

            _uploadServiceMock.Setup(upload => upload.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync((UploadResult?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Image.UploadFailed"); 

            _categoryRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_And_ReplaceImage_When_NewImageProvided()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            using var fakeStream = new MemoryStream(new byte[] { 1, 2, 3 });
            var imageDto = new ImageUploadDto(fakeStream, "new_image.png");

            var command = new UpdateCategoryCommand(categoryId, "Valid Arabic", "Valid English", 1, imageDto);

            var existingCategory = new Domain.Entities.Category
            {
                Id = categoryId,
                NameAr = "Old Arabic",
                NameEn = "Old English",
                OrderIndex = 0,
                ImageUrl = "old_url",
                ImagePublicId = "old_public_id"
            };

            var uploadResult = new UploadResult("http://example.com/fake-image.png", "new_public_id");

            _categoryRepoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<CategoryByNameSpecification>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync((Domain.Entities.Category?)null);

            _categoryRepoMock.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            _uploadServiceMock.Setup(upload => upload.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _uploadServiceMock.Verify(upload => upload.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

            _categoryRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Domain.Entities.Category>(c =>
                c.ImageUrl == uploadResult.Url &&
                c.ImagePublicId == uploadResult.PublicId),
                It.IsAny<CancellationToken>()), Times.Once);

            _uploadServiceMock.Verify(upload => upload.DeleteFileAsync("old_public_id"), Times.Once);
        }
    }
}