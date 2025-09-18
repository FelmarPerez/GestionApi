using GestionApi.Controllers;
using GestionApi.Models;
using GestionApi.Tests.Utilities;
using GestionApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GestionApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserExists()
        {
            var context = DbContextFactory.CreateInMemoryContext();
            context.Usuario.Add(new Usuario { NombreUsuario = "juan", Email = "a@a.com", Password = "x" });
            await context.SaveChangesAsync();

            var jwt = new Mock<IJwtService>();
            var pwd = new Mock<IPasswordService>();
            var controller = new AuthController(context, jwt.Object, pwd.Object);

            var result = await controller.Register(new RegisterRequest { NombreUsuario = "juan", Email = "a@a.com", Password = "123" });

            Assert.IsType<BadRequestObjectResult>(result);
            DbContextFactory.Destroy(context);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalid()
        {
            var context = DbContextFactory.CreateInMemoryContext();
            var jwt = new Mock<IJwtService>();
            var pwd = new Mock<IPasswordService>();
            pwd.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var controller = new AuthController(context, jwt.Object, pwd.Object);

            var result = await controller.Login(new LoginRequest { NombreUsuario = "no", Password = "x" });

            Assert.IsType<UnauthorizedObjectResult>(result);
            DbContextFactory.Destroy(context);
        }
    }
}


