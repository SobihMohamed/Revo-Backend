using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Revo.IntegrationTests.Features.Category
{
    [Collection("SharedTestCollection")]
    public class UpdateCategoryTests : BaseIntegrationTest
    {
        public UpdateCategoryTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        // 1. Update without new image
        [Fact]
        public async Task Update_WhenValidDataWithoutImage_ShouldReturn200OK_AndUpdateDatabase()
        {
            // Arrange 
            var existingCategory = new Domain.Entities.Category
            {
                NameAr = "تصنيف قديم",
                NameEn = "Old Category",
                OrderIndex = 1,
                ImageUrl = "https://old-url.com/img.jpg",
                ImagePublicId = "old_id"
            };
            await _context.Categories.AddAsync(existingCategory);
            await _context.SaveChangesAsync();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("تصنيف متحدث"), "NameAr");
            formData.Add(new StringContent("Updated Category"), "NameEn");
            formData.Add(new StringContent("2"), "OrderIndex");

            // Act
            var response = await HttpClient.PutAsync($"api/categories/{existingCategory.Id}", formData);
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseText}");

            var updatedCategory = await _context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == existingCategory.Id);

            updatedCategory.Should().NotBeNull();
            updatedCategory!.NameAr.Should().Be("تصنيف متحدث");
            updatedCategory.NameEn.Should().Be("Updated Category");
            updatedCategory.OrderIndex.Should().Be(2);
            updatedCategory.ImageUrl.Should().Be("https://old-url.com/img.jpg");
        }
        // 2. Update WITH new image
        [Fact]
        public async Task Update_WhenValidDataWithNewImage_ShouldReturn200OK_AndUpdateImage()
        {
            // Arrange 
            var existingCategory = new Domain.Entities.Category
            {
                NameAr = "تصنيف قديم",
                NameEn = "Old Category",
                OrderIndex = 1,
                ImageUrl = "https://old-url.com/img.jpg",
                ImagePublicId = "old_id"
            };
            await _context.Categories.AddAsync(existingCategory);
            await _context.SaveChangesAsync();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("تصنيف متحدث"), "NameAr");
            formData.Add(new StringContent("Updated Category"), "NameEn");
            formData.Add(new StringContent("2"), "OrderIndex");

            var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF };
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "Image", "new-category.jpg");

            // Act
            var response = await HttpClient.PutAsync($"api/categories/{existingCategory.Id}", formData);
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseText}");

            var updatedCategory = await _context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == existingCategory.Id);

            updatedCategory.Should().NotBeNull();
            updatedCategory!.ImageUrl.Should().Be("https://fakeuploadservice.com/new-category.jpg");
        }

        // 3. Duplicate Name Validation
        [Fact]
        public async Task Update_WhenNameBelongsToAnotherCategory_ShouldReturnBadRequest()
        {
            // Arrange: نزرع تصنيفين
            var category1 = new Domain.Entities.Category { Id = Guid.NewGuid(), NameAr = "تصنيف 1", NameEn = "Cat 1", OrderIndex = 1, ImageUrl = "url", ImagePublicId = "id" };
            var category2 = new Domain.Entities.Category { Id = Guid.NewGuid(), NameAr = "تصنيف 2", NameEn = "Cat 2", OrderIndex = 2, ImageUrl = "url", ImagePublicId = "id" };

            await _context.Categories.AddRangeAsync(category1, category2);
            await _context.SaveChangesAsync();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("تصنيف 1"), "NameAr");
            formData.Add(new StringContent("Cat 2"), "NameEn");
            formData.Add(new StringContent("1"), "OrderIndex");

            // Act
            var response = await HttpClient.PutAsync($"api/categories/{category1.Id}", formData);
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest, because: $"Expected 400 : {responseText}");
            responseText.Should().Contain("Category.DuplicateName");
        }

        // 4. Not Found
        [Fact]
        public async Task Update_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("تصنيف"), "NameAr");
            formData.Add(new StringContent("Category"), "NameEn");
            formData.Add(new StringContent("1"), "OrderIndex");

            // Act
            var response = await HttpClient.PutAsync($"api/categories/{randomId}", formData);
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404 : {responseText}");
            responseText.Should().Contain("Category.NotFound");
        }
    }
}
