using MediatR;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.ContactRequests.Commands;
using Revo.Application.Features.ContactRequests.Events;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.ContactRequests.Commands.Create
{
    public class CreateContactRequestCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<ContactRequest>> _contactRequestRepoMock;
        private readonly Mock<IPublisher> _publisherMock;
        private readonly CreateContactRequestCommandHandler _handler;

        public CreateContactRequestCommandHandlerTests()
        {
            _contactRequestRepoMock = new Mock<IGenericRepo<ContactRequest>>();
            _publisherMock = new Mock<IPublisher>();
            _handler = new CreateContactRequestCommandHandler(_contactRequestRepoMock.Object, _publisherMock.Object);
        }

        [Fact]
        public async Task Handle_Should_AddContactRequest_And_PublishEvent_When_DataIsValid()
        {
            
            // Arrange
            var command = new CreateContactRequestCommand(
                Name: "أحمد محمد",
                PhoneNumber: "01012345678",
                Message: "استفسار عن الخدمة",
                ServiceId: Guid.NewGuid()
            );

            _contactRequestRepoMock.Setup(repo => repo.AddAsync(It.IsAny<ContactRequest>(), It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(new ContactRequest());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _contactRequestRepoMock.Verify(repo => repo.AddAsync(It.IsAny<ContactRequest>()
            , It.IsAny<CancellationToken>()), Times.Once);

            _publisherMock.Verify(pub => pub.Publish(It.Is<ContactRequestCreatedEvent>(e =>
                e.ContactRequest.Name == command.Name
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}