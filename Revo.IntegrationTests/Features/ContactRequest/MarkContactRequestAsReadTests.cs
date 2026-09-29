using FluentAssertions;

using Revo.IntegrationTests.Infrastructre;
using System.Net;
using Microsoft.EntityFrameworkCore;
namespace Revo.IntegrationTests.Features.ContactRequest
{
    [Collection("SharedTestCollection")]
    public class MarkContactRequestAsReadTests
        : BaseIntegrationTest
    {
        public MarkContactRequestAsReadTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }
        [Fact]
        public async Task MarkAsRead_WhenRequestExists_ShouldReturn200OK_AndUpdateDatabase()
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
            var response = await HttpClient.PatchAsync($"api/ContactRequests/{contactRequestId}/mark-as-read", null);

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response was: {responseText}");

            var updatedRequest = await _context.ContactRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == contactRequestId);

            updatedRequest.Should().NotBeNull();
            updatedRequest!.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task MarkAsRead_WhenRequestDoesNotExist_ShouldReturn404NotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var response = await HttpClient.PatchAsync($"api/ContactRequests/{randomId}/mark-as-read", null);

            // Assert
            var responseText = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: $"Response was: {responseText}");
        }
    }
}
