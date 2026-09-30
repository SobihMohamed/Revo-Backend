using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Revo.IntegrationTests.Features.Service
{
    [Collection("SharedTestCollection")]
    public class DeleteServiceTests : BaseIntegrationTest
    {
        public DeleteServiceTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        // 1. Happy Path: Service exists and deletes successfully
        [Fact]
        public async Task Delete_WhenServiceExists_ShouldReturn200OK_AndRemoveFromDatabase()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var serviceToDelete = new Domain.Entities.Service
            {
                Id = serviceId,
                NameAr = "خدمة للحذف",
                NameEn = "Service to delete",
                DescriptionAr = "وصف",
                DescriptionEn = "Desc",
                OrderIndex = 1,
                ImageUrl = "https://fakeuploadservice.com/image.jpg",
                ImagePublicId = "fake_public_id" 
            };

            await _context.Services.AddAsync(serviceToDelete);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.DeleteAsync($"api/services/{serviceId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK : {responseText}");

            var serviceInDb = await _context.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == serviceId);
            serviceInDb.Should().BeNull();
        }

        // 2. Negative Path: Service does not exist
        [Fact]
        public async Task Delete_WhenServiceDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid(); 

            // Act
            var response = await HttpClient.DeleteAsync($"api/services/{randomId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404 NotFound : {responseText}");
            responseText.Should().Contain("ServiceNotFound");
        }
    }
}