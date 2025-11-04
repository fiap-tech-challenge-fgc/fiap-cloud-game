using FCG.Domain.Entities;

namespace FCG.Application.Interfaces.Repository;

public interface ICartRepository
{
    Task<Cart?> GetByPlayerIdAsync(Guid playerId);
    Task<Cart?> GetByFindPlayerIdAsync(Guid playerId);
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
    Task RemoveAsync(Guid cartId);
}

