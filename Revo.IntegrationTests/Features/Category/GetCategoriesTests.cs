using FluentAssertions;
using Revo.API.Resposes;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.Categories.Dto;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Revo.IntegrationTests.Features.Category
{
    [Collection("SharedTestCollection")]
    public class GetCategoriesTests : BaseIntegrationTest
    {
        public GetCategoriesTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        // 1. Get All - Only Active Categories
        [Fact]
        public async Task GetAll_ShouldReturn200OK_AndExcludeDeletedCategories()
        {
            var activeCat1 = new Domain.Entities.Category { Id = Guid.NewGuid(), NameAr = "نشط 1", NameEn = "Active 1", IsDeleted = false, ImageUrl = "url", ImagePublicId = "id" };
            var activeCat2 = new Domain.Entities.Category { Id = Guid.NewGuid(), NameAr = "نشط 2", NameEn = "Active 2", IsDeleted = false, ImageUrl = "url", ImagePublicId = "id" };
            var deletedCat = new Domain.Entities.Category { Id = Guid.NewGuid(), NameAr = "محذوف", NameEn = "Deleted", IsDeleted = true, ImageUrl = "url", ImagePublicId = "id" };

            await _context.Categories.AddRangeAsync(activeCat1, activeCat2, deletedCat);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.GetAsync("api/categories?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<PaginationResponse<CategoryDto>>>();

            responseData.Should().NotBeNull();
            responseData!.IsSuccess.Should().BeTrue();
            responseData.Data.Should().NotBeNull();

            responseData.Data!.Data.Should().Contain(c => c.NameEn == "Active 1");
            responseData.Data.Data.Should().NotContain(c => c.NameEn == "Deleted");
        }

        // 2. Get By Id - Happy Path with Portfolio Items
        [Fact]
        public async Task GetById_WhenCategoryExists_ShouldReturn200OK_WithDetails()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Domain.Entities.Category
            {
                Id = categoryId,
                NameAr = "تصنيف بالمشاريع",
                NameEn = "Category With Items",
                IsDeleted = false,
                ImageUrl = "url",
                ImagePublicId = "id",
                PortfolioItems = new List<Domain.Entities.PortfolioItem>
                {
                    new Domain.Entities.PortfolioItem { Id = Guid.NewGuid(), CaptionAr = "مشروع 1", CaptionEn = "Item 1" },
                    new Domain.Entities.PortfolioItem { Id = Guid.NewGuid(), CaptionAr = "مشروع 2", CaptionEn = "Item 2" }
                }
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.GetAsync($"api/categories/{categoryId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDetailsDto>>();

            responseData.Should().NotBeNull();
            responseData!.IsSuccess.Should().BeTrue();
            responseData.Data.Should().NotBeNull();
            responseData.Data!.Id.Should().Be(categoryId);
            responseData.Data.NameEn.Should().Be("Category With Items");
            responseData.Data.Items.TotalCount.Should().Be(2);
        }

        // 3. Get By Id - Deleted Category
        [Fact]
        public async Task GetById_WhenCategoryIsDeleted_ShouldReturnNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var deletedCategory = new Domain.Entities.Category
            {
                Id = categoryId,
                NameAr = "محذوف",
                NameEn = "Deleted",
                IsDeleted = true, 
                ImageUrl = "url",
                ImagePublicId = "id"
            };

            await _context.Categories.AddAsync(deletedCategory);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.GetAsync($"api/categories/{categoryId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404 : {responseText}");
            responseText.Should().Contain("Category.NotFound");
        }
    }
}