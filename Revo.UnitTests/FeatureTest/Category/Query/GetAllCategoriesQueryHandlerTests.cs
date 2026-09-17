using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Categories.Dto;
using Revo.Application.Features.Categories.Queries.GetAll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Category.Query
{
    public class GetAllCategoriesQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Category>> _repoMock;
        private readonly GetAllCategoriesQueryHandler _handler;

        public GetAllCategoriesQueryHandlerTests()
        {
            _repoMock = new Mock<IGenericRepo<Domain.Entities.Category>>();
            _handler = new GetAllCategoriesQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnPaginatedResponse_With_Data()
        {
            // Arrange
            var query = new GetAllCategoriesQuery(PageIndex: 1, PageSize: 10);

            var expectedDtos = new List<CategoryDto>
            {
                new CategoryDto(Guid.NewGuid(), "Category 1", "Category 1", "url1", 1, 5),
                new CategoryDto(Guid.NewGuid(), "Category 2", "Category 2", "url2", 2, 0)
            };

            _repoMock.Setup(r => r.CountAsync(It.IsAny<ISpecification<Domain.Entities.Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            _repoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Domain.Entities.Category, CategoryDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.PageIndex);
            Assert.Equal(10, result.Value.PageSize);
            Assert.Equal(2, result.Value.TotalCount);
            Assert.Equal(2, result.Value.Data.Count);

            _repoMock.Verify(r => r.CountAsync(It.IsAny<ISpecification<Domain.Entities.Category>>(), It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.Verify(r => r.ListAsync(It.IsAny<ISpecification<Domain.Entities.Category, CategoryDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyPaginationResponse_When_NoCategoriesExist()
        {
            // Arrange
            var query = new GetAllCategoriesQuery(1, 10);

            _repoMock.Setup(r => r.CountAsync(It.IsAny<ISpecification<Domain.Entities.Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0); 

            _repoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Domain.Entities.Category, CategoryDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CategoryDto>()); 

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value!.TotalCount);
            Assert.Empty(result.Value.Data);
            Assert.False(result.Value.HasNextPage);
        }
    }
}