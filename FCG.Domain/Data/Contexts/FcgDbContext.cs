using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Domain.Data.Contexts;

public class FcgDbContext : DbContext
{
    public FcgDbContext(DbContextOptions<FcgDbContext> options)
        : base(options) { }

    public DbSet<Game> Games { get; set; }
    public DbSet<GalleryGame> GalleryGames { get; set; }
    public DbSet<LibraryGame> LibraryGames { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("fcg");

        // TPT: cada classe com sua própria tabela
        builder.Entity<Game>().ToTable("Games");
        builder.Entity<GalleryGame>().ToTable("GalleryGames").HasBaseType<Game>();
        builder.Entity<LibraryGame>().ToTable("LibraryGames").HasBaseType<Game>();

        // Game
        builder.Entity<Game>(game =>
        {
            game.HasKey(g => g.Id);
            game.HasIndex(g => g.EAN).IsUnique();
        });

        // GalleryGame
        builder.Entity<GalleryGame>(gallery =>
        {
            gallery.OwnsOne(g => g.Promotion, promocao =>
            {
                promocao.Property(p => p.Type).HasColumnName("Type");
                promocao.Property(p => p.Value).HasColumnName("Value");
                promocao.Property(p => p.StartOf).HasColumnName("StartOf");
                promocao.Property(p => p.EndOf).HasColumnName("EndOf");
            });
        });

        // Player
        builder.Entity<Player>(player =>
        {
            player.HasKey(p => p.Id);
            player.HasIndex(p => p.UserId).IsUnique();
        });

        // LibraryGame
        builder.Entity<LibraryGame>(library =>
        {
            library.HasOne(l => l.Player)
                .WithMany(p => p.Library) // usa a coleção da classe Player
                .HasForeignKey(l => l.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cart
        builder.Entity<Cart>(cart =>
        {
            cart.HasKey(c => c.Id);

            cart.HasOne(c => c.Player)
                .WithMany()
                .HasForeignKey(c => c.PlayerId);

            cart.HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId);
        });

        // CartItem
        builder.Entity<CartItem>(item =>
        {
            item.HasKey(i => i.Id);

            item.HasIndex(i => new { i.CartId, i.GameId }).IsUnique();

            item.HasOne(i => i.Game)
                .WithMany()
                .HasForeignKey(i => i.GameId);

            item.HasOne(i => i.Player)
                .WithMany()
                .HasForeignKey(i => i.PlayerId);
        });
    }
}