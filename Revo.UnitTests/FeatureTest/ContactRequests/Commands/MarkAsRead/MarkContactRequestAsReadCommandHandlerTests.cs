using Moq;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.ContactRequests.Commands.MarkAsRead;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.ContactRequests.Commands.MarkAsRead
{
    public class MarkContactRequestAsReadCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<ContactRequest>> _contactRequestRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly MarkContactRequestAsReadCommandHandler _handler;

        public MarkContactRequestAsReadCommandHandlerTests()
        {
            _contactRequestRepoMock = new Mock<IGenericRepo<ContactRequest>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new MarkContactRequestAsReadCommandHandler(_contactRequestRepoMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ContactRequestDoesNotExist()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var command = new MarkContactRequestAsReadCommand(invalidId);

            _contactRequestRepoMock
                .Setup(repo => repo.GetByIdAsync(invalidId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ContactRequest?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ContactRequest.NotFound", result.Error.Code);

            _contactRequestRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<ContactRequest>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_And_NotCallDatabase_When_AlreadyRead()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var command = new MarkContactRequestAsReadCommand(requestId);
            var existingRequest = new ContactRequest { Id = requestId, IsRead = true };

            _contactRequestRepoMock
                .Setup(repo => repo.GetByIdAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRequest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _contactRequestRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<ContactRequest>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_MarkAsRead_And_SaveChanges_When_RequestIsNotRead()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var command = new MarkContactRequestAsReadCommand(requestId);
            var existingRequest = new ContactRequest { Id = requestId, IsRead = false }; 

            _contactRequestRepoMock
                .Setup(repo => repo.GetByIdAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRequest);

            _contactRequestRepoMock
                .Setup(repo => repo.UpdateAsync(existingRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChanges(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1); 

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(existingRequest.IsRead);

            _contactRequestRepoMock.Verify(repo => repo.UpdateAsync(existingRequest, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}