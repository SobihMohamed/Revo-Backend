using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Dto;
using Revo.Application.Features.PortfolioItems.Commands.Create;
using Revo.Domain.Entities;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Commands.Create
{
    public class CreatePortfolioItemCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Category>> _categoryRepoMock;
        private readonly CreatePortfolioItemCommandHandler _handler;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepo<PortfolioItem>> _portfolioItemRepoMock;
        private readonly Mock<IUploadService> _uploadServiceMock;
        public CreatePortfolioItemCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _portfolioItemRepoMock = new Mock<IGenericRepo<PortfolioItem>>();
            _categoryRepoMock = new Mock<IGenericRepo<Domain.Entities.Category>>();
            _uploadServiceMock = new Mock<IUploadService>();
            _handler = new CreatePortfolioItemCommandHandler(
                            _categoryRepoMock.Object,
                            _portfolioItemRepoMock.Object,
                            _uploadServiceMock.Object,
                            _unitOfWorkMock.Object);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CategoryDoesNotExist()
        {
            // Arrange
            var command = CreateDummyCommand();
            _categoryRepoMock.Setup(repo => repo.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Category)null!);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("CategoryNotFound", result.Error.Code);
            _uploadServiceMock.Verify(uploadService => uploadService.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_AllMediaUploadedAndSaved()
        {
            // Arrange
            var command = CreateDummyCommand();
            _categoryRepoMock.Setup(repo => repo.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Category { Id = command.CategoryId });
            _uploadServiceMock.Setup(uploadService => uploadService.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(new UploadResult("https://example.com/uploadedfile.png", "publicId"));
            // act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert

            Assert.True(result.IsSuccess);
            _portfolioItemRepoMock.Verify(repo => repo.AddAsync(It.IsAny<PortfolioItem>(), It.IsAny<CancellationToken>()), Times.Once);
            _uploadServiceMock.Verify(uploadService => uploadService.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.IsAny<IEnumerable<string>>()), Times.Never);

        }
        private CreatePortfolioItemCommand CreateDummyCommand()
        {
            var dummyStream = new MemoryStream();

            var mediaItems = new List<CreatePortfolioMediaCommandItem>
            {
                new CreatePortfolioMediaCommandItem(MediaType.Image, 1, new ImageUploadDto(dummyStream, "img.png"), null, null),
                new CreatePortfolioMediaCommandItem(MediaType.Video, 2, null, "https://vimeo.com/123", new ImageUploadDto(dummyStream, "cover.png"))
            };

            return new CreatePortfolioItemCommand("عربي", "English", 1, Guid.NewGuid(), mediaItems);
        }
        [Fact]
        public async Task Handle_Should_RollbackUploadedFiles_When_OneMediaUploadFails()
        {
            // Arrange
            var command = CreateDummyCommand();
            var category = new Domain.Entities.Category { Id = command.CategoryId, IsDeleted = false };

            _categoryRepoMock.Setup(r => r.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);
            _uploadServiceMock.SetupSequence(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(new UploadResult("https://example.com/uploadedfile1.png", "publicId1"))
                .ReturnsAsync((UploadResult?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("MediaUploadFailed", result.Error.Code);
            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.IsAny<IEnumerable<string>>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task Handle_Should_RollbackUploadedFiles_When_DatabaseThrowsException()
        {
            // Arrange
            var command = CreateDummyCommand();
            var category = new Domain.Entities.Category { Id = command.CategoryId, IsDeleted = false };
           
            _categoryRepoMock.Setup(r => r.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);
            _uploadServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(new UploadResult("https://example.com/uploadedfile.png", "publicId"));
            _unitOfWorkMock.Setup(u => u.SaveChanges(It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("Database connection lost"));

            var result = await _handler.Handle(command, CancellationToken.None);
          
            Assert.Equal("System.Error", result.Error.Code);
            _uploadServiceMock.Verify(s => s.DeleteFilesAsync(It.IsAny<IEnumerable<string>>()), Times.Once);
        }
    }
}
