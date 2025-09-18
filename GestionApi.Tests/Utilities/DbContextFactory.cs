using GestionApi.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionApi.Tests.Utilities
{
    public static class DbContextFactory
    {
        public static GestionDBContext CreateInMemoryContext(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<GestionDBContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new GestionDBContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static void Destroy(GestionDBContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}


