using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Revo.IntegrationTests.Features.Category
{
    [Collection("SharedTestCollection")]
    public class DeleteCategoryTests : BaseIntegrationTest
    {
        public DeleteCategoryTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        // 1. Happy Path: Empty Category -> Soft Delete
        [Fact]
        public async Task Delete_WhenCategoryHasNoItems_ShouldReturn200OK_AndSoftDelete()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Domain.Entities.Category
            {
                Id = categoryId,
                NameAr = "تصنيف فاضي",
                NameEn = "Empty Category",
                OrderIndex = 1,
                IsDeleted = false, 
                ImageUrl = "url",
                ImagePublicId = "old_public_id"
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.DeleteAsync($"api/categories/{categoryId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseText}");

            var categoryInDb = await _context.Categories.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoryId);
            categoryInDb.Should().NotBeNull(); 
            categoryInDb!.IsDeleted.Should().BeTrue(); 
        }

        // 2. Business Logic: Category Has Items -> Block Deletion
        [Fact]
        public async Task Delete_WhenCategoryHasPortfolioItems_ShouldReturnBadRequest()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Domain.Entities.Category
            {
                Id = categoryId,
                NameAr = "تصنيف مليان",
                NameEn = "Full Category",
                OrderIndex = 1,
                IsDeleted = false,
                PortfolioItems = new List<Domain.Entities.PortfolioItem>
                {
                    new Domain.Entities.PortfolioItem
                    {
                        Id = Guid.NewGuid(), 
                        CaptionEn = "Test Item",
                        CaptionAr = "عنصر تست",
                        OrderIndex = 1,
                        CategoryId = categoryId
                        
                    }
                }
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.DeleteAsync($"api/categories/{categoryId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest, because: $"Expected 400 : {responseText}");
            responseText.Should().Contain("Category.HasPortfolioItems");

            var categoryInDb = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoryId);
            categoryInDb!.IsDeleted.Should().BeFalse();
        }

        // 3. Not Found
        [Fact]
        public async Task Delete_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var response = await HttpClient.DeleteAsync($"api/categories/{randomId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404 : {responseText}");
            responseText.Should().Contain("Category.NotFound");
        }
    }
}