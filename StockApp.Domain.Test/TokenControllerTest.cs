using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;

namespace StockApp.Domain.Test
{
    public class TokenControllerTest
    {
        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);

            authServiceMock.Setup(service => service.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TokenResponseDto
            {
                Token = "token",
                Expiration = DateTime.UtcNow.AddMinutes(60)
            });

            var userLoginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "password"
            };

            var result = await tokenController.Login(userLoginDto) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<TokenResponseDto>(result.Value);
        }
    }
}
