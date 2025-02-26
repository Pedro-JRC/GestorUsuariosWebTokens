using Microsoft.EntityFrameworkCore;
using GestorUsuariosWebTokens.Models;

namespace GestorUsuariosWebTokens.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar el índice único para la propiedad 'Correo'
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
