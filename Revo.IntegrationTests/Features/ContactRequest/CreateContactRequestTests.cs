using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Revo.API.Requests.ContactRequest;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.IntegrationTests.Infrastructre;
using System.Net;
using System.Net.Http.Json;

namespace Revo.IntegrationTests.Features.ContactRequest
{
    public class CreateContactRequestTests : BaseIntegrationTest
    {
        public CreateContactRequestTests(CustomWebApplicationFactory factory) : base(factory)
        {

        }
        [Fact]
        public async Task Create_WhenDataIsValid_ShouldReturn200OK_AndSaveToDatabase()
        {
            // Arrange 
            var apiRequest = new CreateContactRequestApiRequest
            (
                Name: "Sobieh Mohamed",
                PhoneNumber: "01012345678",
                Message: "Hello from full integration test!",
                ServiceId: null
            );

            //Act 
            var response = await HttpClient.PostAsJsonAsync("api/ContactRequests", apiRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apiResult = await response.Content.ReadFromJsonAsync<ApiResponse<ActionResponse>>();
            apiResult.Should().NotBeNull();
            apiResult.IsSuccess.Should().BeTrue();
            apiResult.Data.Should().NotBeNull();
            // go to db check if the data is saved
            var savedData = await _context.ContactRequests
                .FirstOrDefaultAsync(x => x.Id == apiResult.Data.Id);
            savedData.Should().NotBeNull();
            savedData.Name.Should().Be(apiRequest.Name);
        }
    }
}
