using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Categories.Dto;
using Revo.Application.Features.Categories.Queries.GetById;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Category.Query
{
    public class GetCategoryDetailsQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Category>> _categoryRepoMock;
        private readonly Mock<IGenericRepo<Domain.Entities.PortfolioItem>> _portfolioItemRepoMock;
        private readonly GetCategoryDetailsQueryHandler _handler;

        public GetCategoryDetailsQueryHandlerTests()
        {
            _categoryRepoMock = new Mock<IGenericRepo<Domain.Entities.Category>>();
            _portfolioItemRepoMock = new Mock<IGenericRepo<Domain.Entities.PortfolioItem>>();

            _handler = new GetCategoryDetailsQueryHandler(
                _categoryRepoMock.Object,
                _portfolioItemRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CategoryNotFound_Or_Deleted()
        {
            // Arrange
            var query = new GetCategoryDetailsQuery(Guid.NewGuid());

            // نرجع null كأن التصنيف مش موجود
            _categoryRepoMock.Setup(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Category?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Category.NotFound", result.Error.Code);

            _portfolioItemRepoMock.Verify(r => r.CountAsync(It.IsAny<ISpecification<Domain.Entities.PortfolioItem>>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task Handle_Should_ReturnCategoryDetails_With_PaginatedItems_When_CategoryIsValid()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var query = new GetCategoryDetailsQuery(categoryId, PageIndex: 1, PageSize: 2);

            // 1. الداتا الوهمية للـ Category
            var fakeCategory = new Domain.Entities.Category
            {
                Id = categoryId,
                NameAr = "تصنيف تجريبي",
                NameEn = "Test Category",
                ImageUrl = "http://test.com/img.png",
                OrderIndex = 1,
                IsDeleted = false
            };

            var fakeTotalCount = 5;

            var fakeSnippets = new List<PortfolioItemSnippetDto>
            {
                new PortfolioItemSnippetDto(Guid.NewGuid(), "عنصر 1", "Item 1", 1, "img1.png"),
                new PortfolioItemSnippetDto(Guid.NewGuid(), "عنصر 2", "Item 2", 2, "img2.png")
            };

            // -- Setup Mocks --

            // Mock Get Category
            _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeCategory);

            // Mock Count Items (بتاخد Specification من نوع PortfolioItem)
            _portfolioItemRepoMock.Setup(r => r.CountAsync(It.IsAny<ISpecification<Domain.Entities.PortfolioItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeTotalCount);

            // Mock List Items (بتاخد Specification بيحول من PortfolioItem لـ PortfolioItemSnippetDto)
            _portfolioItemRepoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Domain.Entities.PortfolioItem, PortfolioItemSnippetDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeSnippets);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            Assert.Equal(categoryId, result.Value.Id);
            Assert.Equal(fakeCategory.NameAr, result.Value.NameAr);

            Assert.Equal(fakeTotalCount, result.Value.Items.TotalCount);
            Assert.Equal(2, result.Value.Items.Data.Count); 
            Assert.Equal(3, result.Value.Items.TotalPages); 
            Assert.True(result.Value.Items.HasNextPage); 

            _categoryRepoMock.Verify(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            _portfolioItemRepoMock.Verify(r => r.CountAsync(It.IsAny<ISpecification<Domain.Entities.PortfolioItem>>(), It.IsAny<CancellationToken>()), Times.Once);
            _portfolioItemRepoMock.Verify(r => r.ListAsync(It.IsAny<ISpecification<Domain.Entities.PortfolioItem, PortfolioItemSnippetDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
