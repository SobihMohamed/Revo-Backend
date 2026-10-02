using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.Domain.Enums;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Revo.IntegrationTests.Features.Portfolio
{
    [Collection("SharedTestCollection")]
    public class UpdatePortfolioItemTests : BaseIntegrationTest
    {
        public UpdatePortfolioItemTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        // 1. The Ultimate Happy Path (Update + Delete Old + Add New)
        [Fact]
        public async Task Update_WhenModifyingAndDeletingAndAddingMedia_ShouldHandleAllCorrectly()
        {
            // Arrangex
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

            var portfolioId = Guid.NewGuid();
            var oldMedia1 = new Domain.Entities.PortfolioMedia {
                Id = Guid.NewGuid(),
                Type = MediaType.Image,
                MediaUrl = "old-img",
                MediaPublicId = "old-id1",
                OrderIndex = 1
            };
            var oldMedia2 = new Domain.Entities.PortfolioMedia { 
                Id = Guid.NewGuid(), 
                Type = MediaType.Video,
                MediaUrl = "old-vid", 
                MediaPublicId = "old-id2", 
                OrderIndex = 2 
            };

            var portfolioItem = new Domain.Entities.PortfolioItem
            {
                Id = portfolioId,
                CategoryId = categoryId,
                CaptionAr = "قديم",
                CaptionEn = "Old",
                OrderIndex = 1,
                MediaItems = new List<Domain.Entities.PortfolioMedia> { oldMedia1, oldMedia2 }
            };
            await _context.PortfolioItems.AddAsync(portfolioItem);
            await _context.SaveChangesAsync();

            //  Act 
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("تحديث شامل"), "CaptionAr");
            formData.Add(new StringContent("Massive Update"), "CaptionEn");
            formData.Add(new StringContent("5"), "OrderIndex");
            formData.Add(new StringContent(categoryId.ToString()), "CategoryId");

            formData.Add(new StringContent(oldMedia1.Id.ToString()), "MediaItems[0].Id");
            formData.Add(new StringContent("1"), "MediaItems[0].Type"); 
            formData.Add(new StringContent("10"), "MediaItems[0].OrderIndex");

            // we don't include oldMedia2 in the form data, which means it should be deleted from the database

            // Adding a new media item (image)
            formData.Add(new StringContent("1"), "MediaItems[1].Type"); 
            formData.Add(new StringContent("20"), "MediaItems[1].OrderIndex"); 

            var newImageContent = new ByteArrayContent(new byte[] { 0xFF, 0xD8 });
            newImageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(newImageContent, "MediaItems[1].File", "brand-new.jpg");

            var response = await HttpClient.PutAsync($"api/portfolioitems/{portfolioId}", formData);
            var responseText = await response.Content.ReadAsStringAsync();

            // ================= Assert =================
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseText}");

            var updatedPortfolio = await _context.PortfolioItems
                .Include(p => p.MediaItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == portfolioId);

            updatedPortfolio.Should().NotBeNull();
            updatedPortfolio!.CaptionEn.Should().Be("Massive Update");
            updatedPortfolio.OrderIndex.Should().Be(5);

            updatedPortfolio.MediaItems.Should().HaveCount(2);

            updatedPortfolio.MediaItems.Should().Contain(m => m.Id == oldMedia1.Id && m.OrderIndex == 10);

            updatedPortfolio.MediaItems.Should().NotContain(m => m.Id == oldMedia2.Id);

            updatedPortfolio.MediaItems.Should().Contain(m => m.MediaUrl.Contains("fake") && m.OrderIndex == 20);
        }

        // 2. Negative Path: Item Not Found
        [Fact]
        public async Task Update_WhenItemDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("تيست"), "CaptionAr");
            formData.Add(new StringContent("Test"), "CaptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");
            formData.Add(new StringContent(Guid.NewGuid().ToString()), "CategoryId");
            formData.Add(new StringContent("1"), "MediaItems[0].Type"); // 1 = Image
            formData.Add(new StringContent("1"), "MediaItems[0].OrderIndex");
            var newImageContent = new ByteArrayContent(new byte[] { 0xFF, 0xD8 });
            newImageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(newImageContent, "MediaItems[0].File", "brand-new.jpg");
            // Act
            var response = await HttpClient.PutAsync($"api/portfolioitems/{randomId}", formData);
            var responseText = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404 : {responseText}");
            responseText.Should().Contain("PortfolioItemNotFound");
        }
    }
}