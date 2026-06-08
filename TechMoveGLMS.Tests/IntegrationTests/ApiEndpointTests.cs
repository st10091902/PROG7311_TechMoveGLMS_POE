using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace TechMoveGLMS.Tests.IntegrationTests
{
    public class ApiEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;

            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-API-KEY", "TechMove-Part3-Demo-Key");
        }

        [Fact]
        public async Task GetClients_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("/api/clients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(json));
        }

        [Fact]
        public async Task GetContracts_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(json));
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("/api/service-requests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(json));
        }

        [Fact]
        public async Task GetClients_WithoutApiKey_ReturnsUnauthorized()
        {
            var unauthorizedClient = _factory.CreateClient();

            var response = await unauthorizedClient.GetAsync("/api/clients");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}