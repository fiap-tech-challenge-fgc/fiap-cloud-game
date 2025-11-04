using FCG.Application.Interfaces.Repository;
using FCG.Domain.Data;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Application.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IDAL<Cart> _dal;

        public CartRepository(IDAL<Cart> dal)
        {
            _dal = dal;
        }

        public async Task<Cart?> GetByPlayerIdAsync(Guid playerId)
        {
            // Usa Query() para ter mais controle sobre o Include
            // Carrega Cart -> Items -> Game para acessar as informações do jogo
            var cart = await _dal.Query(c => c.Items)
                .Where(c => c.PlayerId == playerId)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Game)
                .FirstOrDefaultAsync();
            
            return cart;
        }
        public async Task<Cart?> GetByFindPlayerIdAsync(Guid playerId)
        {
            return await _dal.FindAsync(c => c.PlayerId == playerId, c => c.Items, c => c.Items.Select(i => i.Game));
        }

        public async Task AddAsync(Cart cart)
        {
            await _dal.AddAsync(cart);
        }

        public async Task UpdateAsync(Cart cart)
        {
            await _dal.UpdateAsync(cart);
        }

        public async Task RemoveAsync(Guid cartId)
        {
            var cart = await _dal.FindAsync(c => c.Id == cartId);
            if (cart != null)
                await _dal.DeleteAsync(cart);
        }
    }
}
