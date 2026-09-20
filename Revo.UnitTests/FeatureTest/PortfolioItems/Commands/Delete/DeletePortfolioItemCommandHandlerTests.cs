using Ardalis.Specification;
using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.PortfolioItems.Commands.Delete;
using Revo.Domain.Entities;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Commands.Delete
{
    public class DeletePortfolioItemCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<PortfolioItem>> _portfolioRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUploadService> _uploadServiceMock;
        private readonly DeletePortfolioItemCommandHandler _handler;

        public DeletePortfolioItemCommandHandlerTests()
        {
            _portfolioRepoMock = new Mock<IGenericRepo<PortfolioItem>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _uploadServiceMock = new Mock<IUploadService>();

            _handler = new DeletePortfolioItemCommandHandler(
                _portfolioRepoMock.Object,
                _unitOfWorkMock.Object,
                _uploadServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ItemDoesNotExist()
        {
            // Arrange
            var command = new DeletePortfolioItemCommand(Guid.NewGuid());

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PortfolioItem?)null); 

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("PortfolioItemNotFound", result.Error.Code);

            _portfolioRepoMock.Verify(r => r.DeleteAsync(It.IsAny<PortfolioItem>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_DeleteFromDbAndCloudinary_When_ItemHasMedia()
        {
            // Arrange
            var command = new DeletePortfolioItemCommand(Guid.NewGuid());

            var existingItem = new PortfolioItem
            {
                Id = command.Id,
                MediaItems = new List<PortfolioMedia>
                {
                    new PortfolioMedia { MediaPublicId = "image_123" },
                    new PortfolioMedia { MediaPublicId = "video_456", CoverImagePublicId = "cover_789" }
                }
            };

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _portfolioRepoMock.Verify(r => r.DeleteAsync(existingItem), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);

            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.Is<IEnumerable<string>>(ids =>
                ids.Contains("image_123") &&
                ids.Contains("video_456") &&
                ids.Contains("cover_789") &&
                ids.Count() == 3
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_DeleteFromDbOnly_When_ItemHasNoMedia()
        {
            // Arrange
            var command = new DeletePortfolioItemCommand(Guid.NewGuid());

            var existingItem = new PortfolioItem
            {
                Id = command.Id,
                MediaItems = new List<PortfolioMedia>() 
            };

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);


            _portfolioRepoMock.Verify(r => r.DeleteAsync(existingItem), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);

            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.IsAny<IEnumerable<string>>()), Times.Never);
        }
        [Fact]
        public async Task Handle_Should_NotDeleteFromCloudinary_When_DbSaveChangesThrowsException()
        {
            // Arrange
            var command = new DeletePortfolioItemCommand(Guid.NewGuid());
            var existingItem = new PortfolioItem
            {
                Id = command.Id,
                MediaItems = new List<PortfolioMedia> { new PortfolioMedia { MediaPublicId = "image_123" } }
            };

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            _unitOfWorkMock.Setup(u => u.SaveChanges(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.IsAny<IEnumerable<string>>()), Times.Never);
        }
    }
}