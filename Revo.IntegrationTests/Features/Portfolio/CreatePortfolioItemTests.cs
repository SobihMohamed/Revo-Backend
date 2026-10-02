using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Revo.Domain.Enums;
using Revo.IntegrationTests.Infrastructre;
using System.Net;
using System.Net.Http.Headers;

namespace Revo.IntegrationTests.Features.Portfolio
{
    [Collection("SharedTestCollection")]
    public class CreatePortfolioItemTests : BaseIntegrationTest
    {
        public CreatePortfolioItemTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        // 1. Happy Path: Create Item with Image Media
        [Fact]
        public async Task Create_WhenValidDataWithImage_ShouldReturn200OK_AndSaveToDatabase()
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

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("مشروع جديد"), "CaptionAr");
            formData.Add(new StringContent("New Item"), "CaptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");
            formData.Add(new StringContent(categoryId.ToString()), "CategoryId");

            formData.Add(new StringContent("1"), "MediaItems[0].Type"); 
            formData.Add(new StringContent("1"), "MediaItems[0].OrderIndex");

            var imageContent = new ByteArrayContent(new byte[] { 0xFF, 0xD8, 0xFF });
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "MediaItems[0].File", "portfolio-img.jpg");

            // Act
            var response = await HttpClient.PostAsync("api/portfolioitems", formData); 
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseText}");

            var portfolioItemInDb = await _context.PortfolioItems
                .Include(p => p.MediaItems) 
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CaptionEn == "New Item");

            portfolioItemInDb.Should().NotBeNull();
            portfolioItemInDb!.CategoryId.Should().Be(categoryId);
            portfolioItemInDb.MediaItems.Should().HaveCount(1);
            portfolioItemInDb.MediaItems.First().OrderIndex.Should().Be(1);
            portfolioItemInDb.MediaItems.First().Type.Should().Be(Domain.Enums.MediaType.Image);
            portfolioItemInDb.MediaItems.First().MediaUrl.Should().Contain("fake");

        }
        // 2. Happy Path: Create Item with Video & Cover
        [Fact]
        public async Task Create_WhenValidDataWithVideoAndCover_ShouldReturn200OK_AndSaveToDatabase()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Domain.Entities.Category {
                Id = categoryId,
                NameAr = "فيديو",
                NameEn = "Vid Cat",
                IsDeleted = false, 
                ImageUrl = "url",
                ImagePublicId = "id" 
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("فيديو جديد"), "CaptionAr");
            formData.Add(new StringContent("New Video"), "CaptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");
            formData.Add(new StringContent(categoryId.ToString()), "CategoryId");

            formData.Add(new StringContent("Video"), "MediaItems[0].Type");
            formData.Add(new StringContent("1"), "MediaItems[0].OrderIndex");
            formData.Add(new StringContent("https://youtube.com/watch?v=123"), "MediaItems[0].VideoUrl");

            var coverContent = new ByteArrayContent(new byte[] { 0xFF, 0xD8 });
            coverContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(coverContent, "MediaItems[0].CoverImage", "cover.jpg");

            // Act
            var response = await HttpClient.PostAsync("api/portfolioitems", formData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var portfolioItemInDb = await _context.PortfolioItems
                .Include(p => p.MediaItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CaptionEn == "New Video");

            portfolioItemInDb!.MediaItems.First().Type.Should().Be(Domain.Enums.MediaType.Video);
            portfolioItemInDb.MediaItems.First().MediaUrl.Should().Be("https://youtube.com/watch?v=123");
            portfolioItemInDb.MediaItems.First().CoverImageUrl.Should().Contain("fake"); 
        }
        // 3. Negative Path: Category Is Deleted or Not Found
        [Fact]
        public async Task Create_WhenCategoryIsDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var deletedCategory = new Domain.Entities.Category {
                Id = categoryId, 
                NameAr = "محذوف", 
                NameEn = "Deleted",
                IsDeleted = true,
                ImageUrl = "url",
                ImagePublicId = "id"
            };
            await _context.Categories.AddAsync(deletedCategory);
            await _context.SaveChangesAsync();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("تيست"), "CaptionAr");
            formData.Add(new StringContent("Test"), "CaptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");
            formData.Add(new StringContent(categoryId.ToString()), "CategoryId");

            formData.Add(new StringContent("1"), "MediaItems[0].Type");
            var imageContent = new ByteArrayContent(new byte[] { 0x1 });
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "MediaItems[0].File", "img.jpg");

            // Act
            var response = await HttpClient.PostAsync("api/portfolioitems", formData);
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404 : {responseText}");
            responseText.Should().Contain("CategoryNotFound");
        }
    }
}