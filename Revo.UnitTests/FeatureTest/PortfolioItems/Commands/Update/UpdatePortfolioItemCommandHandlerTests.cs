using Ardalis.Specification;
using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Dto;
using Revo.Application.Features.PortfolioItems.Commands.Update;
using Revo.Domain.Entities;
using Revo.Domain.Enums;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Commands.Update
{
    public class UpdatePortfolioItemCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<PortfolioItem>> _portfolioRepoMock;
        private readonly Mock<IUploadService> _uploadServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdatePortfolioItemCommandHandler _handler;
        public UpdatePortfolioItemCommandHandlerTests()
        {
            _portfolioRepoMock = new Mock<IGenericRepo<PortfolioItem>>();
            _uploadServiceMock = new Mock<IUploadService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdatePortfolioItemCommandHandler(_portfolioRepoMock.Object, _uploadServiceMock.Object, _unitOfWorkMock.Object);
        }
        // =================================================================
        // 1. Test: Item Not Found
        // =================================================================
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_PortfolioItemNotFound()
        {
            // Arrange
            var command = new UpdatePortfolioItemCommand(Guid.NewGuid(), "Ar", "En", 1, Guid.NewGuid(), new List<UpdatePortfolioMediaCommandItem>());
            _portfolioRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PortfolioItem)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("PortfolioItemNotFound", result.Error.Code);
        }
        // =================================================================
        // 2. Test: Success Delta Pattern (Add, Update, Delete)
        // =================================================================
        [Fact]
        public async Task Handle_Should_ProcessDeltaCorrectly_When_ValidRequest()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var keptMediaId = Guid.NewGuid();
            var deletedMediaId = Guid.NewGuid();
            // database has two media items: one to keep and one to delete
            var existingItem = new PortfolioItem
            {
                Id = portfolioId,
                MediaItems = new List<PortfolioMedia>
                {
                    new PortfolioMedia { Id = keptMediaId, MediaPublicId = "kept_public_id" },
                    new PortfolioMedia { Id = deletedMediaId, MediaPublicId = "deleted_public_id" }
                }
            };

            var dummyFile = new ImageUploadDto(new MemoryStream(), "new.png");
            var command = new UpdatePortfolioItemCommand(portfolioId, "Ar", "En", 1, Guid.NewGuid(),
                new List<UpdatePortfolioMediaCommandItem>
                {
                    new UpdatePortfolioMediaCommandItem(keptMediaId, MediaType.Image, 1, null, null, null),
                    new UpdatePortfolioMediaCommandItem(null, MediaType.Image, 2, null, dummyFile, null) 
                });

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            _uploadServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UploadResult("http://new-url.com", "new_public_id"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _portfolioRepoMock.Verify(r => r.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);

            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.Is<IEnumerable<string>>(ids => ids.Contains("deleted_public_id"))), Times.Once);
        }
        // =================================================================
        // 3. Test: Upload Failure
        // =================================================================
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_NewMediaUploadFails()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var existingItem = new PortfolioItem { Id = portfolioId, MediaItems = new List<PortfolioMedia>() };

            var dummyFile = new ImageUploadDto(new MemoryStream(), "fail.png");
            var command = new UpdatePortfolioItemCommand(portfolioId, "Ar", "En", 1, Guid.NewGuid(),
                new List<UpdatePortfolioMediaCommandItem>
                {
                    new UpdatePortfolioMediaCommandItem(null, MediaType.Image, 1, null, dummyFile, null)
                });

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);
            _uploadServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UploadResult?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("MediaUploadFailed", result.Error.Code);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
        }
        // =================================================================
        // 4. Test: Database Exception (Rollback)
        // =================================================================
        [Fact]
        public async Task Handle_Should_RollbackUploadedMedia_When_DatabaseThrowsException()
        {
            // Arrange
            var portfolioId = Guid.NewGuid();
            var existingItem = new PortfolioItem { Id = portfolioId, MediaItems = new List<PortfolioMedia>() };

            var dummyFile = new ImageUploadDto(new MemoryStream(), "new.png");
            var command = new UpdatePortfolioItemCommand(portfolioId, "Ar", "En", 1, Guid.NewGuid(),
                new List<UpdatePortfolioMediaCommandItem>
                {
                    new UpdatePortfolioMediaCommandItem(null, MediaType.Image, 1, null, dummyFile, null)
                });

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            _uploadServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UploadResult("url", "new_public_id_to_rollback"));

            _unitOfWorkMock.Setup(u => u.SaveChanges(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection lost"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("System.Error", result.Error.Code);

            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.Is<IEnumerable<string>>(ids => ids.Contains("new_public_id_to_rollback"))), Times.Once);
        }
    }
}
