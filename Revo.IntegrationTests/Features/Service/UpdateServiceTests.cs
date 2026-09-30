using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.API.Requests.Service;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Revo.IntegrationTests.Features.Service
{
    [Collection("SharedTestCollection")]
    public class UpdateServiceTests : BaseIntegrationTest
    {
        public UpdateServiceTests(CustomWebApplicationFactory factory) : base(factory)
        {

        }
        // 1. Update without new image
        [Fact]
        public async Task Update_WhenValidDataWithoutImage_ShouldReturn200OK_AndUpdateDatabase()
        {
            // Arrange 
            var existingService = new Domain.Entities.Service
            {
                NameAr = "خدمة موجودة",
                NameEn = "Existing Service",
                DescriptionAr = "وصف",
                DescriptionEn = "Desc",
                OrderIndex = 1,
                ImageUrl = "url",
                ImagePublicId = "id"
            };
            await _context.Services.AddAsync(existingService);
            await _context.SaveChangesAsync();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("خدمة التحديث"), "NameAr");
            formData.Add(new StringContent("updated Service"), "NameEn");
            formData.Add(new StringContent("وصف التحديث"), "DescriptionAr");
            formData.Add(new StringContent(" updated description in English"), "DescriptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");

            // Act
            var response = await HttpClient.PutAsync($"api/services/{existingService.Id}", formData);
            var responseText = await response.Content.ReadAsStringAsync();
            //Assert
            var updatedService = await _context.Services.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == existingService.Id);

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(updatedService);
            Assert.Equal("خدمة التحديث", updatedService.NameAr);
            Assert.Equal("updated Service", updatedService.NameEn);
            Assert.Equal("وصف التحديث", updatedService.DescriptionAr);
            Assert.Equal(" updated description in English", updatedService.DescriptionEn);
            Assert.Equal(1, updatedService.OrderIndex);
            Assert.Equal("url", updatedService.ImageUrl);
            Assert.Equal("id", updatedService.ImagePublicId);
        }
        // 2. Update WITH new image
        [Fact]
        public async Task Update_WhenValidDataWithNewImage_ShouldReturn200OK_AndUpdateImage()
        {
            // Arrange 
            var existingService = new Domain.Entities.Service
            {
                NameAr = "خدمة موجودة",
                NameEn = "Existing Service",
                DescriptionAr = "وصف",
                DescriptionEn = "Desc",
                OrderIndex = 1,
                ImageUrl = "https://fakeuploadservice.com/image.jpg",
                ImagePublicId = "id"
            };
            await _context.Services.AddAsync(existingService);
            await _context.SaveChangesAsync();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("خدمة التحديث"), "NameAr");
            formData.Add(new StringContent("updated Service"), "NameEn");
            formData.Add(new StringContent("وصف التحديث"), "DescriptionAr");
            formData.Add(new StringContent(" updated description in English"), "DescriptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");
            var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF }; // Mock JPEG header bytes
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "Image", "test-image.jpg");
            // Act
            var response = await HttpClient.PutAsync($"api/services/{existingService.Id}", formData);
            var responseText = await response.Content.ReadAsStringAsync();
            //Assert
            var updatedService = await _context.Services.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == existingService.Id);

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(updatedService);
            Assert.Equal("خدمة التحديث", updatedService.NameAr);
            Assert.Equal("updated Service", updatedService.NameEn);
            Assert.Equal("وصف التحديث", updatedService.DescriptionAr);
            Assert.Equal(" updated description in English", updatedService.DescriptionEn);
            Assert.Equal(1, updatedService.OrderIndex);
            Assert.Equal("https://fakeuploadservice.com/test-image.jpg", updatedService.ImageUrl);
        }

        // 3. Duplicate Name Validation
        [Fact]
        public async Task Update_WhenNameBelongsToAnotherService_ShouldReturnBadRequest()
        {
            // Arrange 
            var service1 = new Domain.Entities.Service
            {
                Id = Guid.NewGuid(),
                NameAr = "الخدمة الأولى",
                NameEn = "First Service",
                DescriptionAr = "وصف 1",
                DescriptionEn = "Desc 1",
                OrderIndex = 1
            };

            var service2 = new Domain.Entities.Service
            {
                Id = Guid.NewGuid(),
                NameAr = "الخدمة الثانية",
                NameEn = "Second Service", 
                DescriptionAr = "وصف 2",
                DescriptionEn = "Desc 2",
                OrderIndex = 2
            };
            await _context.Services.AddRangeAsync(service1, service2);
            await _context.SaveChangesAsync();
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("الخدمة الثانية"), "NameAr");
            formData.Add(new StringContent("Existing Service"), "NameEn");
            formData.Add(new StringContent("وصف التحديث"), "DescriptionAr");
            formData.Add(new StringContent(" updated description in English"), "DescriptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");
            // Act
            var response = await HttpClient.PutAsync($"api/services/{service1.Id}", formData);
            var responseText = await response.Content.ReadAsStringAsync();
            //Assert

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseText.Should().Contain("Service.DuplicateName");
        }
        // 4. Not Found
        [Fact]
        public async Task Update_WhenServiceDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("أي اسم"), "NameAr");
            formData.Add(new StringContent("Any Name"), "NameEn");
            formData.Add(new StringContent("وصف"), "DescriptionAr");
            formData.Add(new StringContent("Desc"), "DescriptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");

            // Act
            var response = await HttpClient.PutAsync($"api/services/{randomId}", formData);

            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404 : {responseText}");
        }
    }
}
