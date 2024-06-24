using Microsoft.Extensions.Configuration;
using Moq;
using StockApp.Application.DTOs;
using StockApp.Application.Services;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

namespace StockApp.Domain.Test
{
    public class UnitTest
    {
        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsToken()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var configurationMock = new Mock<IConfiguration>();
            var authService = new AuthService(userRepositoryMock.Object, configurationMock.Object);

            userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User
            {
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Role = "User"
            });

            configurationMock.Setup(config => config["Jwt:Key"]).Returns("ChaveSecretaParaJwtToken");
            configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("SeuIssuer");
            configurationMock.Setup(config => config["Jwt:Audience"]).Returns("SuaAudience");
            configurationMock.Setup(config => config["Jwt:ExpireMinutes"]).Returns("15");

            var result = await authService.AuthenticateAsync("testuser", "password");

            Assert.NotNull(result);
            Assert.IsType<TokenResponseDto>(result);
        }
    }
}