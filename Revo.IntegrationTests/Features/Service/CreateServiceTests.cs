using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.IntegrationTests.Infrastructre;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace Revo.IntegrationTests.Features.Service
{
    [Collection("SharedTestCollection")]
    public class CreateServiceTests : BaseIntegrationTest
    {
        public CreateServiceTests(CustomWebApplicationFactory factory) : base(factory)
        {

        }
        // 1. Integration Test - Happy Path (Database Integration & File Mocking)
        [Fact]
        public async Task Create_WhenDataIsValid_ShouldReturn200OK_AndSaveToDatabase()
        {
            // Arrange 
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("خدمة التنظيف"), "NameAr");
            formData.Add(new StringContent("Cleaning Service"), "NameEn");
            formData.Add(new StringContent("وصف بالعربي"), "DescriptionAr");
            formData.Add(new StringContent("Description in English"), "DescriptionEn");
            formData.Add(new StringContent("1"), "OrderIndex");
            var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF }; // Mock JPEG header bytes
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "Image", "test-image.jpg");
            // Act
            var response = await HttpClient.PostAsync("api/services", formData);
            var responseContent = await response.Content.ReadAsStringAsync();
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK, because: $"Expected 200 OK : {responseContent}");

            var apiResult = await response.Content.ReadFromJsonAsync<ApiResponse<ActionResponse>>();
            // check the Database 
            var serviceId = apiResult?.Data?.Id;
            var serviceInDb = await _context.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == serviceId);
            serviceInDb.Should().NotBeNull();
            serviceInDb!.NameAr.Should().Be("خدمة التنظيف");
            serviceInDb.ImageUrl.Should().Be("https://fakeuploadservice.com/test-image.jpg");
        }
        // 2. Integration Test - Database Unique Constraint (ServiceByNameSpecification)
        [Fact]
        public async Task Create_WhenNameAlreadyExists_ShouldReturnBadRequest()
        {
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
            formData.Add(new StringContent("اسم عربي مختلف"), "NameAr");
            formData.Add(new StringContent("Existing Service"), "NameEn"); 
            formData.Add(new StringContent("Desc"), "DescriptionAr");
            formData.Add(new StringContent("Desc"), "DescriptionEn");
            formData.Add(new StringContent("2"), "OrderIndex");

            var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF }; // Mock JPEG header bytes
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "Image", "test-image.jpg");

            // 2. Act
            var response = await HttpClient.PostAsync("api/Services", formData);

            // 3. Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest,because: $"Expected 400 BadRequest : {responseText}");
            responseText.Should().Contain("Service.DuplicateName");
        }
    }
}
