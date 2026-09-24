using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Notifications.Dtos;
using Revo.Application.Features.Notifications.Helper;
using Revo.Application.Features.Notifications.Queries.GetAll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Notification.Queries.GetAll
{
    public class GetNotificationsQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Notification>> _notificationRepoMock;
        private readonly GetNotificationsQueryHandler _handler;

        public GetNotificationsQueryHandlerTests()
        {
            _notificationRepoMock = new Mock<IGenericRepo<Domain.Entities.Notification>>();
            _handler = new GetNotificationsQueryHandler(_notificationRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnPaginationResponse_When_Called()
        {
            // Arrange
            var queryParams = new NotificationSpecParams { PageIndex = 1, PageSize = 10, IsRead = null };
            var query = new GetNotificationsQuery(queryParams);

            var expectedList = new List<NotificationDto>
            {
                new NotificationDto { Id = System.Guid.NewGuid(), TitleAr = "إشعار 1" },
                new NotificationDto { Id = System.Guid.NewGuid(), TitleAr = "إشعار 2" }
            };
            int expectedTotalCount = 2;

            _notificationRepoMock
                .Setup(repo => repo.ListAsync(It.IsAny<ISpecification<Domain.Entities.Notification, NotificationDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            _notificationRepoMock
                .Setup(repo => repo.CountAsync(It.IsAny<ISpecification<Domain.Entities.Notification>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTotalCount);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedTotalCount, result.Value.TotalCount);
            Assert.Equal(expectedList.Count, result.Value.Data.Count);

            _notificationRepoMock.Verify(repo =>
                repo.ListAsync(It.IsAny<ISpecification<Domain.Entities.Notification, NotificationDto>>(), It.IsAny<CancellationToken>()), Times.Once);

            _notificationRepoMock.Verify(repo =>
                repo.CountAsync(It.IsAny<ISpecification<Domain.Entities.Notification>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}