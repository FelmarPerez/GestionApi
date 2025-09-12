using GestionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionApi.Data
{
    public class GestionDBContext:DbContext

    {

        public GestionDBContext(DbContextOptions<GestionDBContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Tareas> Task { get; set; } 

    }
}
