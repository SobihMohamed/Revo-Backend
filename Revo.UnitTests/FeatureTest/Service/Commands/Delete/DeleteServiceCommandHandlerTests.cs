using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Services.Commands.Delete;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Service.Commands.Delete
{
    public class DeleteServiceCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Service>> _serviceRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUploadService> _uploadServiceMock;
        private readonly DeleteServiceCommandHandler _handler;

        public DeleteServiceCommandHandlerTests()
        {
            _serviceRepoMock = new Mock<IGenericRepo<Domain.Entities.Service>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _uploadServiceMock = new Mock<IUploadService>();

            _handler = new DeleteServiceCommandHandler(
                _serviceRepoMock.Object,
                _unitOfWorkMock.Object,
                _uploadServiceMock.Object);
        }

        [Fact]  
        public async Task Handle_Should_ReturnFailure_When_ServiceDoesNotExist()
        {
            // Arrange
            var command = new DeleteServiceCommand(Guid.NewGuid());
            _serviceRepoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Service?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ServiceNotFound", result.Error.Code);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_And_NotDeleteImage_When_DbDeleteFails()
        {
            // Arrange
            var command = new DeleteServiceCommand(Guid.NewGuid());
            var service = new Domain.Entities.Service { Id = command.Id, ImagePublicId = "pic_to_keep" };

            _serviceRepoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            _unitOfWorkMock.Setup(u => u.SaveChanges(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection lost"));
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ServiceDeleteFailed", result.Error.Code);
            _uploadServiceMock.Verify(u => u.DeleteFileAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_DeleteService_And_DeleteImage_When_Successful()
        {
            // Arrange
            var command = new DeleteServiceCommand(Guid.NewGuid());
            var service = new Domain.Entities.Service { Id = command.Id, ImagePublicId = "pic_to_delete_123" };

            _serviceRepoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _serviceRepoMock.Verify(r => r.DeleteAsync(service, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);

            _uploadServiceMock.Verify(u => u.DeleteFileAsync("pic_to_delete_123"), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_DeleteService_And_NotCallUploadService_When_ImagePublicIdIsEmpty()
        {
            // Arrange
            var command = new DeleteServiceCommand(Guid.NewGuid());
            var service = new Domain.Entities.Service { Id = command.Id, ImagePublicId = string.Empty };

            _serviceRepoMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            _serviceRepoMock.Setup(r => r.DeleteAsync(service))
                .ReturnsAsync(1); 

            _unitOfWorkMock.Setup(u => u.SaveChanges(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1); 

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess, result.Error?.Message);

            _serviceRepoMock.Verify(r => r.DeleteAsync(service), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);

            _uploadServiceMock.Verify(u => u.DeleteFileAsync(It.IsAny<string>()), Times.Never);
        }
    }
}