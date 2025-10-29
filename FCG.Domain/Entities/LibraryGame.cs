namespace FCG.Domain.Entities;

public class LibraryGame : Game
{
    public Guid PlayerId { get; private set; }
    public Player Player { get; private set; } = null!;
    public DateTime PurchaseDate { get; private set; }
    public decimal PurchasePrice { get; private set; }

    private LibraryGame() : base() { }

    public LibraryGame(Game game, Player player, decimal purchasePrice) 
        : base(game.EAN, game.Name, game.Genre, game.Description)
    {
        if (purchasePrice < 0) 
            throw new ArgumentException("Preço de compra inválido");

        Id = Guid.NewGuid();
        PlayerId = player.Id;
        Player = player;
        PurchaseDate = DateTime.UtcNow;
        PurchasePrice = purchasePrice;
    }

    public override string GetDescription()
    {
        return $"{base.GetDescription()} - Comprado em {PurchaseDate:d} por {PurchasePrice:C}";
    }
}