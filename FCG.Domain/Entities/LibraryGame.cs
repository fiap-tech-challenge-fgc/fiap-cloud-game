namespace FCG.Domain.Entities;

public class LibraryGame
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public Game Game { get; private set; } = null!;
    public Guid PlayerId { get; private set; }
    public Player Player { get; private set; } = null!;
    public DateTime PurchaseDate { get; private set; }
    public decimal PurchasePrice { get; private set; }

    protected LibraryGame() { }

    public LibraryGame(Game game, Player player, decimal purchasePrice)
    {
        if (purchasePrice < 0)
            throw new ArgumentException("Preço de compra inválido");

        Id = Guid.NewGuid();
        GameId = game.Id;
        Game = game;
        PlayerId = player.Id;
        Player = player;
        PurchaseDate = DateTime.UtcNow;
        PurchasePrice = purchasePrice;
    }

    public string GetDescription()
        => $"{Game.GetDescription()} - Comprado em {PurchaseDate:d} por {PurchasePrice:C}";
}
