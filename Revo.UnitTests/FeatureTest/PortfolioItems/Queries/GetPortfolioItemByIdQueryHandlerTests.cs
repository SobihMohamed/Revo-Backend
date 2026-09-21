using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.PortfolioItems.Dto;
using Revo.Application.Features.PortfolioItems.Queries.GetById;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Queries
{
    public class GetPortfolioItemByIdQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<PortfolioItem>> _portfolioRepoMock;
        private readonly GetPortfolioItemByIdQueryHandler _handler;

        public GetPortfolioItemByIdQueryHandlerTests()
        {
            _portfolioRepoMock = new Mock<IGenericRepo<PortfolioItem>>();
            _handler = new GetPortfolioItemByIdQueryHandler(_portfolioRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ItemDoesNotExist()
        {
            // Arrange
            var query = new GetPortfolioItemByIdQuery(Guid.NewGuid());

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem, PortfolioItemDetailsDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PortfolioItemDetailsDto?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("PortfolioItemNotFound", result.Error.Code);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessWithData_When_ItemExists()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var query = new GetPortfolioItemByIdQuery(itemId);
            var expectedDto = new PortfolioItemDetailsDto
            {
                Id = itemId,
                CaptionAr = "مشروع ناجح"
            };

            _portfolioRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<PortfolioItem, PortfolioItemDetailsDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(itemId, result.Value.Id);
            Assert.Equal("مشروع ناجح", result.Value.CaptionAr);
        }
    }
}