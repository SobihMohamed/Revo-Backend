using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.Domain.Enums;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Revo.IntegrationTests.Features.Portfolio
{
    [Collection("SharedTestCollection")]
    public class DeletePortfolioItemTests : BaseIntegrationTest
    {
        public DeletePortfolioItemTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        // 1. Happy Path: Delete Item and Cascade its Media
        [Fact]
        public async Task Delete_WhenItemExistsWithMedia_ShouldReturn200OK_AndRemoveFromDatabase()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Domain.Entities.Category {
                Id = categoryId,
                NameAr = "تصنيف",
                NameEn = "Cat",
                IsDeleted = false, 
                ImageUrl = "url",
                ImagePublicId = "id" 
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var portfolioId = Guid.NewGuid();
            var media1 = new Domain.Entities.PortfolioMedia {
                Id = Guid.NewGuid(),
                Type = MediaType.Image,
                MediaUrl = "img1", 
                MediaPublicId = "public_id_1", 
                OrderIndex = 1 };
            var media2 = new Domain.Entities.PortfolioMedia { 
                Id = Guid.NewGuid(), 
                Type = MediaType.Video, 
                MediaUrl = "vid1", 
                CoverImagePublicId = "public_id_cover",
                OrderIndex = 2 
            };

            var portfolioItem = new Domain.Entities.PortfolioItem
            {
                Id = portfolioId,
                CategoryId = categoryId,
                CaptionAr = "مشروع للحذف",
                CaptionEn = "Item to Delete",
                OrderIndex = 1,
                MediaItems = new List<Domain.Entities.PortfolioMedia> { media1, media2 }
            };

            await _context.PortfolioItems.AddAsync(portfolioItem);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.DeleteAsync($"api/portfolioitems/{portfolioId}");
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseText}");

            var itemInDb = await _context.PortfolioItems
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == portfolioId);
            itemInDb.Should().BeNull();

            var anyMediaLeft = await _context.Set<Domain.Entities.PortfolioMedia>()
                .AsNoTracking()
                .AnyAsync(m => m.Id == media1.Id || m.Id == media2.Id);

            anyMediaLeft.Should().BeFalse("Because deleting the portfolio item should cascade delete its media items.");
        }

        // 2. Negative Path: Item Not Found
        [Fact]
        public async Task Delete_WhenItemDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var response = await HttpClient.DeleteAsync($"api/portfolioitems/{randomId}");
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404 : {responseText}");
            responseText.Should().Contain("PortfolioItemNotFound");
        }
    }
}