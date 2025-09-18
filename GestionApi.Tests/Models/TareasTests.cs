using GestionApi.Models;

namespace GestionApi.Tests.Models
{
    public class TareasTests
    {
        [Fact]
        public void Tareas_AssignProperties_Works()
        {
            var task = new Tareas
            {
                Id = 1,
                Description = "Test",
                DueDate = new DateTime(2025, 1, 1),
                Status = "Open",
                AdditionalData = "N/A"
            };

            Assert.Equal(1, task.Id);
            Assert.Equal("Test", task.Description);
            Assert.Equal(new DateTime(2025, 1, 1), task.DueDate);
            Assert.Equal("Open", task.Status);
            Assert.Equal("N/A", task.AdditionalData);
        }
    }
}


