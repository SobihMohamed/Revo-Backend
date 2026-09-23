using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.Application.Features.ContactRequests.Queries.GetById;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.ContactRequests.Queries
{
    public class GetContactRequestByIdQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<ContactRequest>> _contactRequestRepoMock;
        private readonly GetContactRequestByIdQueryHandler _handler;

        public GetContactRequestByIdQueryHandlerTests()
        {
            _contactRequestRepoMock = new Mock<IGenericRepo<ContactRequest>>();
            _handler = new GetContactRequestByIdQueryHandler(_contactRequestRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessWithDto_When_ContactRequestExists()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var query = new GetContactRequestByIdQuery(requestId);

            var expectedDto = new ContactRequestDto
            {
                Id = requestId,
                Name = "Sobieh",
                PhoneNumber = "01000000000",
                Message = "Test Message"
            };

            _contactRequestRepoMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<ISingleResultSpecification<ContactRequest, ContactRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedDto.Id, result.Value.Id);
            Assert.Equal(expectedDto.Name, result.Value.Name);

            _contactRequestRepoMock.Verify(repo => repo.FirstOrDefaultAsync(It.IsAny<ISingleResultSpecification<ContactRequest, ContactRequestDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ContactRequestDoesNotExist()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var query = new GetContactRequestByIdQuery(invalidId);

            _contactRequestRepoMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<ISingleResultSpecification<ContactRequest, ContactRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ContactRequestDto?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("ContactRequest.NotFound", result.Error.Code); 

            _contactRequestRepoMock.Verify(repo => repo.FirstOrDefaultAsync(It.IsAny<ISingleResultSpecification<ContactRequest, ContactRequestDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
