using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Domain.Interfaces;

namespace StockApp.Domain.Test
{
    public class UsersControllerTest
    {
        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var usersController = new UsersController(userRepositoryMock.Object);

            var userRegisterDto = new UserRegisterDto
            {
                Username = "testuser",
                Password = "password",
                Role = "User"
            };

            var result = await usersController.Register(userRegisterDto) as OkResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}
