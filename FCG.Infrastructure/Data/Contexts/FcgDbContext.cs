using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Data.Contexts
{
    public class FcgDbContext : DbContext
    {
        public FcgDbContext(DbContextOptions<FcgDbContext> options)
            : base(options) { }

        // public DbSet<User> Users { get; set; }
        // public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configurações adicionais
        }
    }
}
