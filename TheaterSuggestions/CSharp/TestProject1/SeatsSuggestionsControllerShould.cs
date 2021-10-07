
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using SeatsSuggestions.Api;
using Xunit;

namespace SeatsSuggestions.Tests.SystemTests
{
    public class SeatsSuggestionsControllerShould : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public SeatsSuggestionsControllerShould(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;

        }

        [Fact]
        public async Task s()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/SeatsSuggestions?showId=5&party=3");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/plain; charset=utf-8", 
                         response.Content.Headers.ContentType.ToString());
        }
    }
}