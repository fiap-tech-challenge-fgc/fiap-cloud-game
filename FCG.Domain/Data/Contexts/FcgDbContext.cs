using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Domain.Data.Contexts;

public class FcgDbContext : DbContext
{
    public FcgDbContext(DbContextOptions<FcgDbContext> options)
        : base(options) { }

    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Promotion> Promotion { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("fcg");

        builder.Entity<Game>(game =>
        {
            game.HasKey(g => g.Id);

            game.OwnsOne(g => g.Promotion, promocao =>
            {
                promocao.Property(p => p.Type).HasColumnName("PromocaoTipo");
                promocao.Property(p => p.Value).HasColumnName("PromocaoValor");
                promocao.Property(p => p.StartOf).HasColumnName("PromocaoInicio");
                promocao.Property(p => p.EndOf).HasColumnName("PromocaoFim");
            });
        });

    }
}
