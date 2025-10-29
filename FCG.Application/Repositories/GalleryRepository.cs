using FCG.Application.Interfaces.Repository;
using FCG.Domain.Data;
using FCG.Domain.Data.Contexts;
using FCG.Domain.Entities;
using FCG.Domain.ValuesObjects;

namespace FCG.Application.Repositories;

public class GalleryRepository : IGalleryRepository
{
    private readonly IDAL<GalleryGame> _dal;
    private readonly FcgDbContext _context;

    public GalleryRepository(IDAL<GalleryGame> dal, FcgDbContext context)
    {
        _dal = dal;
        _context = context;
    }

    public async Task<bool> ExistsAsync(string EAN)
    {
        var game = await _dal.FindAsync(g => g.EAN == EAN);
        return game != null;
    }

    public async Task<GalleryGame> AddToGalleryAsync(GalleryGame game)
    {
        await _dal.AddAsync(game);
        await SaveChangesAsync();
        return game;
    }

    public async Task<GalleryGame?> GetGalleryGameByIdAsync(Guid id)
    {
        return await _dal.FindAsync(g => g.Id == id, g => g.Promotion);
    }

    public async Task<IEnumerable<GalleryGame>> GetAllGalleryGamesAsync()
    {
        return await _dal.ListAsync(g => g.Promotion);
    }

    public async Task UpdateGalleryGameAsync(GalleryGame game)
    {
        await _dal.UpdateAsync(game);
        await SaveChangesAsync();
    }

    public async Task RemoveFromGalleryAsync(GalleryGame game)
    {
        await _dal.DeleteAsync(game);
        await SaveChangesAsync();
    }

    public async Task<IEnumerable<GalleryGame>> GetPromotionalGamesAsync()
    {
        return await _dal.FindListAsync(
            g => g.Promotion != null && g.Promotion != Promotion.None,
            g => g.Promotion);
    }

    public async Task<IEnumerable<GalleryGame>> GetGamesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dal.FindListAsync(
            g => g.Price >= minPrice && g.Price <= maxPrice,
            g => g.Promotion);
    }

    public async Task<bool> IsAvailableForPurchaseAsync(Guid id)
    {
        var game = await GetGalleryGameByIdAsync(id);
        return game != null;
    }

    public async Task<IEnumerable<GalleryGame>> GetGamesByGenreAsync(string genre)
    {
        return await _dal.FindListAsync(
            g => g.Genre.ToLower() == genre.ToLower(),
            g => g.Promotion);
    }

    public async Task<IEnumerable<GalleryGame>> SearchGamesAsync(string searchTerm)
    {
        searchTerm = searchTerm.ToLower();
        return await _dal.FindListAsync(
            g => g.Name.ToLower().Contains(searchTerm) || 
                 g.Genre.ToLower().Contains(searchTerm) || 
                 (g.Description != null && g.Description.ToLower().Contains(searchTerm)),
            g => g.Promotion);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}