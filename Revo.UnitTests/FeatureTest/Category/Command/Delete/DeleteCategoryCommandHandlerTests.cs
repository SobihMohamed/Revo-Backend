using Ardalis.Specification;
using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Categories.Commands.Delete;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Revo.UnitTests.FeatureTest.Category.Command.Delete
{
    public class DeleteCategoryCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Category>> _categoryRepoMock;
        private readonly DeleteCategoryCommandHandler _handler;
        private readonly Mock<IUploadService> _uploadServiceMock;

        public DeleteCategoryCommandHandlerTests()
        {
            _categoryRepoMock = new Mock<IGenericRepo<Domain.Entities.Category>>();
            _uploadServiceMock = new Mock<IUploadService>();
            _handler = new DeleteCategoryCommandHandler(_categoryRepoMock.Object, _uploadServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
        {
            // Arrange
            var command = new DeleteCategoryCommand(Guid.NewGuid());

            _categoryRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Domain.Entities.Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Category?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Category.NotFound", result.Error.Code);
            
            _uploadServiceMock.Verify(u => u.DeleteFileAsync(It.IsAny<string>()), Times.Never);
            _categoryRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CategoryHasPortfolioItems()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Domain.Entities.Category
            {
                Id = categoryId,
                PortfolioItems = new List<Domain.Entities.PortfolioItem> { new Domain.Entities.PortfolioItem() }
            };
            var command = new DeleteCategoryCommand(categoryId);

            _categoryRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Domain.Entities.Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Category.HasPortfolioItems", result.Error.Code);

            _uploadServiceMock.Verify(u => u.DeleteFileAsync(It.IsAny<string>()), Times.Never);
            _categoryRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_And_SoftDelete_When_CategoryIsValid()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Domain.Entities.Category
            {
                Id = categoryId,
                PortfolioItems = new List<Domain.Entities.PortfolioItem>(),
                ImagePublicId = "fake-public-id"
            };
            var command = new DeleteCategoryCommand(categoryId);

            _categoryRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Domain.Entities.Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(category.IsDeleted);

            _categoryRepoMock.Verify(r => r.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once);
            _uploadServiceMock.Verify(u => u.DeleteFileAsync("fake-public-id"), Times.Once);
        }
    }
}