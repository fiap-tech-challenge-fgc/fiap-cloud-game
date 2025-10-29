using FCG.Domain.Entities;

namespace FCG.Domain.Service;

public class CartDomainService : ICartDomainService
{
    public Cart CreateCart(Guid playerId)
    {
        if (playerId == Guid.Empty)
            throw new ArgumentException("PlayerId cannot be empty", nameof(playerId));

        return new Cart(playerId);
    }

    public void AddItemToCart(Cart cart, Game game)
    {
        if (cart == null)
            throw new ArgumentNullException(nameof(cart));
        if (game == null)
            throw new ArgumentNullException(nameof(game));

        cart.AddItem(game);
    }

    public void RemoveItemFromCart(Cart cart, Guid gameId)
    {
        if (cart == null)
            throw new ArgumentNullException(nameof(cart));
        if (gameId == Guid.Empty)
            throw new ArgumentException("GameId cannot be empty", nameof(gameId));

        cart.RemoveItem(gameId);
    }

    public void ClearCart(Cart cart)
    {
        if (cart == null)
            throw new ArgumentNullException(nameof(cart));

        cart.Clear();
    }
}