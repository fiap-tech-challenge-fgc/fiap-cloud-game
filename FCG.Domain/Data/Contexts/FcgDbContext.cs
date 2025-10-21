using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Domain.Data.Contexts;

public class FcgDbContext : DbContext
{
    public FcgDbContext(DbContextOptions<FcgDbContext> options)
        : base(options) { }

    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("fcg");

        builder.Entity<Game>(game =>
        {
            game.HasKey(g => g.Id);

            game.OwnsOne(g => g.Promotion, promocao =>
            {
                promocao.Property(p => p.Type).HasColumnName("Type");
                promocao.Property(p => p.Value).HasColumnName("Value");
                promocao.Property(p => p.StartOf).HasColumnName("StartOf");
                promocao.Property(p => p.EndOf).HasColumnName("EndOf");
            });
        });


        builder.Entity<CartItem>()
            .HasIndex(c => new { c.PlayerId, c.GameId })
            .IsUnique();

        builder.Entity<CartItem>()
            .HasOne(c => c.Game)
            .WithMany()
            .HasForeignKey(c => c.GameId);

        builder.Entity<CartItem>()
            .HasOne(c => c.Player)
            .WithMany()
            .HasForeignKey(c => c.PlayerId);

        builder.Entity<Purchase>()
            .HasIndex(p => new { p.PlayerId, p.GameId })
            .IsUnique(); 

        builder.Entity<Purchase>()
            .HasOne(p => p.Game)
            .WithMany()
            .HasForeignKey(p => p.GameId);

        builder.Entity<Purchase>()
            .HasOne(p => p.Player)
            .WithMany()
            .HasForeignKey(p => p.PlayerId);

    }
}
