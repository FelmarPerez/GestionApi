using GestionApi.Controllers;
using GestionApi.Models;
using GestionApi.Tests.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace GestionApi.Tests.Controllers
{
    public class TaskControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOk_WithTasks()
        {
            var context = DbContextFactory.CreateInMemoryContext();
            context.Task.Add(new Tareas { Id = 1, Description = "T1", Status = "Open", AdditionalData = "-" });
            context.Task.Add(new Tareas { Id = 2, Description = "T2", Status = "Open", AdditionalData = "-" });
            await context.SaveChangesAsync();

            var controller = new TaskController(context);

            var result = await controller.Get();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<Tareas>>(ok.Value);
            Assert.Equal(2, list.Count());

            DbContextFactory.Destroy(context);
        }

        [Fact]
        public async Task GetById_NotFound_WhenMissing()
        {
            var context = DbContextFactory.CreateInMemoryContext();
            var controller = new TaskController(context);

            var result = await controller.Get(123);

            Assert.IsType<NotFoundObjectResult>(result);
            DbContextFactory.Destroy(context);
        }

        [Fact]
        public async Task Post_Creates_Task()
        {
            var context = DbContextFactory.CreateInMemoryContext();
            var controller = new TaskController(context);

            var toCreate = new Tareas { Id = 10, Description = "Nueva", Status = "Open", AdditionalData = "-" };

            var result = await controller.Post(toCreate);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var saved = Assert.IsType<Tareas>(created.Value);
            Assert.Equal(10, saved.Id);

            DbContextFactory.Destroy(context);
        }
    }
}


