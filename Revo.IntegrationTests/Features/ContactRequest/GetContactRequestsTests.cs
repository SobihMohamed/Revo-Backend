using FluentAssertions;
using Revo.API.Resposes;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.IntegrationTests.Infrastructre;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Revo.IntegrationTests.Features.ContactRequest
{
    [Collection("SharedTestCollection")]
    public class GetContactRequestsTests : BaseIntegrationTest
    {
        public GetContactRequestsTests(CustomWebApplicationFactory factory) : base(factory)
        {

        }
        // 1. Get By Id - Happy Path
        [Fact]
        public async Task GetById_WhenRequestExists_ShouldReturn200OK_AndCorrectData()
        {
            // Arrange
            var contactRequestId = Guid.NewGuid();
            var newContactRequest = new Domain.Entities.ContactRequest
            {
                Id = contactRequestId,
                Name = "Sobieh",
                PhoneNumber = "01012345678",
                Message = "Test Message",
                IsRead = false
            };
            await _context.ContactRequests.AddAsync(newContactRequest);
            await _context.SaveChangesAsync();
            // Act
            var response = await HttpClient.GetAsync($"api/ContactRequests/{contactRequestId}");
            var responseText = await response.Content.ReadAsStringAsync();
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK,because: $"Expected {responseText}");
            var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<ContactRequestDto>>();

            responseData.Should().NotBeNull();
            responseData!.IsSuccess.Should().BeTrue();
            responseData.Data.Should().NotBeNull();
            responseData.Data!.Id.Should().Be(contactRequestId);
            responseData.Data.Name.Should().Be("Sobieh");

        }
        // 2. Get By Id - Negative Path
        [Fact]
        public async Task GetById_WhenRequestDoesNotExist_ShouldReturn404NotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var response = await HttpClient.GetAsync($"api/ContactRequests/{randomId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        // 3. Get All - Happy Path (Pagination)
        // ==========================================
        [Fact]
        public async Task GetAll_ShouldReturn200OK_AndPaginatedList()
        {
            // Arrange
            var request1 = new Domain.Entities.ContactRequest
            {
                Id = Guid.NewGuid(),
                Name = "User One",
                PhoneNumber = "01000000001",
                Message = "Message 1"
            };
            var request2 = new Domain.Entities.ContactRequest
            {
                Id = Guid.NewGuid(),
                Name = "User Two",
                PhoneNumber = "01000000002",
                Message = "Message 2"
            };

            await _context.ContactRequests.AddRangeAsync(request1, request2);
            await _context.SaveChangesAsync();

            // Act
            var response = await HttpClient.GetAsync("api/ContactRequests?pageIndex=1&pageSize=5");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Read the response content as JSON and deserialize it into ApiResponse<PaginationResponse<ContactRequestDto>>
            var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<PaginationResponse<ContactRequestDto>>>();

            responseData.Should().NotBeNull();
            responseData!.IsSuccess.Should().BeTrue();
            responseData.Data.Should().NotBeNull();

            responseData.Data!.TotalCount.Should().BeGreaterThanOrEqualTo(2);
            responseData.Data.Data.Should().NotBeEmpty();
        }
    }
}
