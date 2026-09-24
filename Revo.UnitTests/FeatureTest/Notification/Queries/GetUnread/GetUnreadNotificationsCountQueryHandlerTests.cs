using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Notifications.Queries.GetUnread;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Notification.Queries.GetUnread
{
    public class GetUnreadNotificationsCountQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Notification>> _notificationRepoMock;
        private readonly GetUnreadNotificationsCountQueryHandler _handler;

        public GetUnreadNotificationsCountQueryHandlerTests()
        {
            _notificationRepoMock = new Mock<IGenericRepo<Domain.Entities.Notification>>();
            _handler = new GetUnreadNotificationsCountQueryHandler(_notificationRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnCorrectUnreadCount_When_Called()
        {
            // Arrange
            var query = new GetUnreadNotificationsCountQuery();
            int expectedCount = 5;

            _notificationRepoMock
                .Setup(repo => repo.CountAsync(It.IsAny<ISpecification<Domain.Entities.Notification>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedCount, result.Value);

            _notificationRepoMock.Verify(repo =>
                repo.CountAsync(It.IsAny<ISpecification<Domain.Entities.Notification>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}