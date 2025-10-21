using FCG.Domain.Entities;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid PlayerId { get; private set; }
    public Guid GameId { get; private set; }

    public Game Game { get; private set; } = null!;
    public Player Player { get; private set; } = null!;

    private CartItem() { }

    public CartItem(Guid playerId, Guid gameId)
    {
        Id = Guid.NewGuid();
        PlayerId = playerId;
        GameId = gameId;
    }
}