using FluentAssertions;
using Revo.API.Resposes;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.Services.Dto;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Revo.IntegrationTests.Features.Service
{
    [Collection("SharedTestCollection")]
    public class GetServicesTests : BaseIntegrationTest
    {
        public GetServicesTests(CustomWebApplicationFactory factory) : base(factory)
        {
            
        }
        // 1. Get By Id - Happy Path
        [Fact]
        public async Task GetById_WhenServiceExists_ShouldReturn200OK_AndCorrectData()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var newService = new Domain.Entities.Service
            {
                Id = serviceId,
                NameAr = "خدمة للبحث",
                NameEn = "Searchable Service",
                DescriptionAr = "وصف",
                DescriptionEn = "Desc",
                OrderIndex = 1,
                ImageUrl = "url",
                ImagePublicId = "id"
            };

            await _context.Services.AddAsync(newService);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.GetAsync($"api/services/{serviceId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK: {responseText}");

            var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<ServiceDto>>();

            responseData.Should().NotBeNull();
            responseData!.IsSuccess.Should().BeTrue();
            responseData.Data.Should().NotBeNull();
            responseData.Data!.Id.Should().Be(serviceId);
            responseData.Data.NameEn.Should().Be("Searchable Service");
        }
        // 2. Get By Id - Negative Path (Not Found)
        [Fact]
        public async Task GetById_WhenServiceDoesNotExist_ShouldReturn404NotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var response = await HttpClient.GetAsync($"api/services/{randomId}");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Expected 404: {responseText}");
            responseText.Should().Contain("Service.NotFound");
        }
        // 3. Get All - Happy Path (Pagination)
        [Fact]
        public async Task GetAll_ShouldReturn200OK_AndPaginatedList()
        {
            var service1 = new Domain.Entities.Service { Id = Guid.NewGuid(), NameAr = "1", NameEn = "S1", DescriptionAr = "1", DescriptionEn = "1", OrderIndex = 1, ImageUrl = "url", ImagePublicId = "id" };
            var service2 = new Domain.Entities.Service { Id = Guid.NewGuid(), NameAr = "2", NameEn = "S2", DescriptionAr = "2", DescriptionEn = "2", OrderIndex = 2, ImageUrl = "url", ImagePublicId = "id" };

            await _context.Services.AddRangeAsync(service1, service2);
            await _context.SaveChangesAsync();

            var response = await HttpClient.GetAsync("api/services?pageIndex=1&pageSize=10");

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Expected 200 OK: {responseText}");

            var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<PaginationResponse<ServiceDto>>>();

            responseData.Should().NotBeNull();
            responseData!.IsSuccess.Should().BeTrue();
            responseData.Data.Should().NotBeNull();
            responseData.Data!.TotalCount.Should().BeGreaterThanOrEqualTo(2);
            responseData.Data.Data.Should().NotBeEmpty();
        }
    }
}
