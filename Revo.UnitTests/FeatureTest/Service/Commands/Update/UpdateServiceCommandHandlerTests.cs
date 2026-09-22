using FluentAssertions;
using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Dto;
using Revo.Application.Features.Services.Commands.Update;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Service.Commands.Update
{
    public class UpdateServiceCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Service>> _serviceRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUploadService> _uploadServiceMock;
        private readonly UpdateServiceCommandHandler _handler;

        public UpdateServiceCommandHandlerTests()
        {
            _serviceRepoMock = new Mock<IGenericRepo<Domain.Entities.Service>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _uploadServiceMock = new Mock<IUploadService>();

            _handler = new UpdateServiceCommandHandler(
                _serviceRepoMock.Object,
                _unitOfWorkMock.Object,
                _uploadServiceMock.Object);
        }

        private ImageUploadDto CreateDummyImageDto()
        {
            var dummyStream = new MemoryStream(new byte[] { 1 });
            return new ImageUploadDto(dummyStream, "dummy.jpg");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ServiceDoesNotExist()
        {
            var command = new UpdateServiceCommand(Guid.NewGuid(), "Ar", "En", "DescAr", "DescEn", 1, null);

            _serviceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Service?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Service.NotFound", result.Error.Code);
        }

        [Fact]
        public async Task Handle_Should_UpdateData_And_NotCallUploadService_When_NoNewImageProvided()
        {
            var existingService = new Domain.Entities.Service { Id = Guid.NewGuid(), ImagePublicId = "old_pic" };
            var command = new UpdateServiceCommand(existingService.Id, "Ar", "En", "DescAr", "DescEn", 1, null);

            _serviceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingService);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _serviceRepoMock.Verify(r => r.UpdateAsync(existingService, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);

            _uploadServiceMock.Verify(u => u.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _uploadServiceMock.Verify(u => u.DeleteFileAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_UploadNewImage_And_DeleteOldImage_When_SaveChangesSucceeds()
        {
            var existingService = new Domain.Entities.Service { Id = Guid.NewGuid(), ImagePublicId = "old_pic_123" };
            var command = new UpdateServiceCommand(existingService.Id, "Ar", "En", "DescAr", "DescEn", 1, CreateDummyImageDto());

            var uploadResult = new UploadResult (  "new_url", "new_pic_456" );

            _serviceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingService);

            _uploadServiceMock.Setup(u => u.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("new_pic_456", existingService.ImagePublicId);
            _uploadServiceMock.Verify(u => u.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _uploadServiceMock.Verify(u => u.DeleteFileAsync("old_pic_123"), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_DeleteNewImage_When_SaveChangesThrowsException()
        {
            var existingService = new Domain.Entities.Service { Id = Guid.NewGuid(), ImagePublicId = "old_pic_123" };
            var command = new UpdateServiceCommand(existingService.Id, "Ar", "En", "DescAr", "DescEn", 1, CreateDummyImageDto());

            var uploadResult = new UploadResult (  "new_url", "new_pic_456" );

            _serviceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingService);

            _uploadServiceMock.Setup(u => u.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResult);

            _unitOfWorkMock.Setup(u => u.SaveChanges(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });

            // Assert
            _uploadServiceMock.Verify(u => u.DeleteFileAsync("new_pic_456"), Times.Once);

            _uploadServiceMock.Verify(u => u.DeleteFileAsync("old_pic_123"), Times.Never);
        }
    }
}