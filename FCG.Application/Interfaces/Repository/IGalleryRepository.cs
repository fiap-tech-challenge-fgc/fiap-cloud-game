using FCG.Domain.Entities;

namespace FCG.Application.Interfaces.Repository;

public interface IGalleryRepository
{
    Task<bool> ExistsAsync(string EAN);
    Task<GalleryGame?> GetGalleryGameByIdAsync(Guid id);
    Task<IEnumerable<GalleryGame>> GetAllGalleryGamesAsync();
    Task<GalleryGame> AddToGalleryAsync(GalleryGame game);
    Task UpdateGalleryGameAsync(GalleryGame game);
    Task RemoveFromGalleryAsync(GalleryGame game);
    
    // Promotion and price operations
    Task<IEnumerable<GalleryGame>> GetPromotionalGamesAsync();
    Task<IEnumerable<GalleryGame>> GetGamesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<bool> IsAvailableForPurchaseAsync(Guid id);
    
    // Filtering and search
    Task<IEnumerable<GalleryGame>> GetGamesByGenreAsync(string genre);
    Task<IEnumerable<GalleryGame>> SearchGamesAsync(string searchTerm);
    
    Task<int> SaveChangesAsync();
}