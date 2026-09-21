using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.PortfolioItems.Dto;
using Revo.Application.Features.PortfolioItems.Queries.GetAll;
using Revo.Application.Features.PortfolioItems.Queries.Helper;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.PortfolioItems.Queries
{
    public class GetAllPortfolioItemsQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<PortfolioItem>> _portfolioRepoMock;
        private readonly GetAllPortfolioItemsQueryHandler _handler;

        public GetAllPortfolioItemsQueryHandlerTests()
        {
            _portfolioRepoMock = new Mock<IGenericRepo<PortfolioItem>>();
            _handler = new GetAllPortfolioItemsQueryHandler(_portfolioRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_And_NotCallListAsync_When_TotalCountIsZero()
        {
            // Arrange
            var specParams = new PortfolioItemSpecParams { PageIndex = 1, PageSize = 10 };
            var query = new GetAllPortfolioItemsQuery(specParams);

            _portfolioRepoMock.Setup(r => r.CountAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(0, result.Value.TotalCount);
            Assert.Empty(result.Value.Data);

            _portfolioRepoMock.Verify(r => r.ListAsync(It.IsAny<ISpecification<PortfolioItem, PortfolioItemListDto>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnPaginationResponseWithData_When_TotalCountIsGreaterThanZero()
        {
            // Arrange
            var specParams = new PortfolioItemSpecParams { PageIndex = 1, PageSize = 10 };
            var query = new GetAllPortfolioItemsQuery(specParams);

            var fakeData = new List<PortfolioItemListDto>
            {
                new PortfolioItemListDto { Id = Guid.NewGuid(), CaptionAr = "Test 1" },
                new PortfolioItemListDto { Id = Guid.NewGuid(), CaptionAr = "Test 2" }
            };

            _portfolioRepoMock.Setup(r => r.CountAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            _portfolioRepoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<PortfolioItem, PortfolioItemListDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeData);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.TotalCount);
            Assert.Equal(2, result.Value.Data.Count);
            Assert.Equal("Test 1", result.Value.Data[0].CaptionAr);

            _portfolioRepoMock.Verify(r => r.CountAsync(It.IsAny<ISpecification<PortfolioItem>>(), It.IsAny<CancellationToken>()), Times.Once);
            _portfolioRepoMock.Verify(r => r.ListAsync(It.IsAny<ISpecification<PortfolioItem, PortfolioItemListDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}