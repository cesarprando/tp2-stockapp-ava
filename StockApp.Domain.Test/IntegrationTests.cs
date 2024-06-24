using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StockApp.Application.DTOs;
using System.Net.Http.Json;

namespace StockApp.Domain.Test
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterAndLogin_ValidCredentials_ReturnsToken()
        {
            var userRegisterDto = new UserRegisterDto
            {
                Username = "testuser",
                Password = "password",
                Role = "User"
            };

            var userLoginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "password"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", userRegisterDto);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsJsonAsync("/api/token/login", userLoginDto);
            loginResponse.EnsureSuccessStatusCode();

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.NotNull(tokenResponse.Token);
            Assert.True(tokenResponse.Expiration > DateTime.UtcNow);
        }
    }
}
