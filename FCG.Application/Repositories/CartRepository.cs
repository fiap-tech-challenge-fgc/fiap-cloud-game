using FCG.Application.Interfaces.Repository;
using FCG.Domain.Data;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Application.Repositories;

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
        // Carrega Cart -> ItemsCollection -> Game para acessar as informações do jogo
        var cart = new Cart(playerId);

        try
        {
            cart = await _dal.Query()
                .Where(c => c.PlayerId == playerId)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Gallery)
                .FirstOrDefaultAsync();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return cart;
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

    public async Task<bool> OwnsItemAsync(Guid playerId, Guid galleryId)
    {
        var cart = await _dal.Query()
            .Where(c => c.PlayerId == playerId &&
                        c.Items.Any(i => i.GalleryId == galleryId))
            .Include(c => c.Items)
                .ThenInclude(i => i.Gallery)
            .FirstOrDefaultAsync();

        return cart != null;
    }
}
