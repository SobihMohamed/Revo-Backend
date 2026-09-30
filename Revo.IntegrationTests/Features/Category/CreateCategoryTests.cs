using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Revo.IntegrationTests.Features.Category
{
    [Collection("SharedTestCollection")]
    public class CreateCategoryTests : BaseIntegrationTest
    {
        public CreateCategoryTests(CustomWebApplicationFactory factory) : base(factory)
        {

        }
        // 1. Happy Path (Database Integration & File Mocking)
        [Fact]
        public async Task Create_WhenDataIsValid_ShouldReturn200OK_AndSaveToDatabase()
        {
            // Arrange 
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("تصنيف جديد"), "NameAr");
            formData.Add(new StringContent("New Category"), "NameEn");
            formData.Add(new StringContent("1"), "OrderIndex");

            var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF }; // Mock JPEG header
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "Image", "category-image.jpg");

            // Act: 
            var response = await HttpClient.PostAsync("api/categories", formData);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseContent}");

            var apiResult = await response.Content.ReadFromJsonAsync<ApiResponse<ActionResponse>>();
            var categoryId = apiResult?.Data?.Id;

            var categoryInDb = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoryId);
            categoryInDb.Should().NotBeNull();
            categoryInDb!.NameAr.Should().Be("تصنيف جديد");
            categoryInDb.ImageUrl.Should().Be("https://fakeuploadservice.com/category-image.jpg");
        }

        // 2. Duplicate Name Validation
        [Fact]
        public async Task Create_WhenNameAlreadyExists_ShouldReturnBadRequest()
        {
            var existingCategory = new Domain.Entities.Category
            {
                NameAr = "تصنيف موجود",
                NameEn = "Existing Category",
                OrderIndex = 1,
                ImageUrl = "url",
                ImagePublicId = "id"
            };
            await _context.Categories.AddAsync(existingCategory);
            await _context.SaveChangesAsync();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("اسم عربي مختلف"), "NameAr");
            formData.Add(new StringContent("Existing Category"), "NameEn"); 
            formData.Add(new StringContent("2"), "OrderIndex");

            var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF };
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "Image", "category-image.jpg");

            // Act
            var response = await HttpClient.PostAsync("api/categories", formData);
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest, because: $"Expected 400 BadRequest : {responseText}");
            responseText.Should().Contain("Category.DuplicateName");
        }
    }
}