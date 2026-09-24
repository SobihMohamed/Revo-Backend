using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Notifications.Commands.MarkAsRead;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Notification.Commands.MarkAsRead
{
    public class MarkNotificationAsReadCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Notification>> _notificationRepoMock;
        private readonly MarkNotificationAsReadCommandHandler _handler;

        public MarkNotificationAsReadCommandHandlerTests()
        {
            _notificationRepoMock = new Mock<IGenericRepo<Domain.Entities.Notification>>();
            _handler = new MarkNotificationAsReadCommandHandler(_notificationRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_And_Update_When_NotificationIsUnread()
        {
            // Arrange
            var notificationId = Guid.NewGuid();
            var command = new MarkNotificationAsReadCommand(notificationId);
            var notification = new Domain.Entities.Notification { Id = notificationId, IsRead = false };

            _notificationRepoMock.Setup(repo => repo.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(notification);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(notification.IsRead);
            _notificationRepoMock.Verify(repo => repo.UpdateAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WithoutUpdating_When_NotificationIsAlreadyRead()
        {
            // Arrange
            var notificationId = Guid.NewGuid();
            var command = new MarkNotificationAsReadCommand(notificationId);
            var notification = new Domain.Entities.Notification { Id = notificationId, IsRead = true };

            _notificationRepoMock.Setup(repo => repo.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(notification);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _notificationRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Notification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_NotificationNotFound()
        {
            // Arrange
            var command = new MarkNotificationAsReadCommand(Guid.NewGuid());

            _notificationRepoMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync((Domain.Entities.Notification)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Notification.NotFound", result.Error.Code); 
            _notificationRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Notification>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}