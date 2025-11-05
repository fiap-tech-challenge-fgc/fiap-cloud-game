using FCG.Domain.Entities;

namespace FCG.Domain.Service;

public interface ICartDomainService
{
    Cart CreateCart(Guid playerId);
    void AddItemToCart(Cart cart, Game game);
    void RemoveItemFromCart(Cart cart, Guid gameId);
    void ClearCart(Cart cart);
}