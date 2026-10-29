using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.Application.Features.ContactRequests.Queries.GetAll;
using Revo.Application.Features.ContactRequests.Queries.Helper;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.ContactRequests.Queries
{
    public class GetAllContactRequestsQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<ContactRequest>> _contactRequestRepoMock;
        private readonly GetAllContactRequestsQueryHandler _handler;

        public GetAllContactRequestsQueryHandlerTests()
        {
            _contactRequestRepoMock = new Mock<IGenericRepo<ContactRequest>>();
            _handler = new GetAllContactRequestsQueryHandler(_contactRequestRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnPaginationResponse_WithCorrectParams()
        {
            // Arrange
            var specParams = new ContactRequestSpecParams
            {
                PageIndex = 2,
                PageSize = 5,
                Search = "010",
                IsRead = false
            };

            var query = new GetAllContactRequestsQuery(specParams);

            var expectedDtoList = new List<ContactRequestDto>
            {
                new ContactRequestDto { Id = Guid.NewGuid(), Name = "Sobieh" },
                new ContactRequestDto { Id = Guid.NewGuid(), Name = "Ahmed" }
            };

            var expectedTotalCount = 12; 

            _contactRequestRepoMock
                .Setup(repo => repo.ListAsync(It.IsAny<ISpecification<ContactRequest, ContactRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDtoList);

            _contactRequestRepoMock
                .Setup(repo => repo.CountAsync(It.IsAny<ISpecification<ContactRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTotalCount);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            Assert.Equal(expectedTotalCount, result.Value.TotalCount);
            Assert.Equal(specParams.PageIndex, result.Value.PageIndex);
            Assert.Equal(specParams.PageSize, result.Value.PageSize);
            Assert.Equal(expectedDtoList.Count, result.Value.Data.Count);

            _contactRequestRepoMock.Verify(repo => repo.ListAsync(It.IsAny<ISpecification<ContactRequest, ContactRequestDto>>(), It.IsAny<CancellationToken>()), Times.Once);
            _contactRequestRepoMock.Verify(repo => repo.CountAsync(It.IsAny<ISpecification<ContactRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}