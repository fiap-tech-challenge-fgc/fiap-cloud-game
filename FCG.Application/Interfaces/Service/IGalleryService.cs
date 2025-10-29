using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;

namespace FCG.Application.Interfaces.Service;

public interface IGalleryService
{
    // Admin operations
    Task<OperationResult<GalleryGameResponseDto>> AddToGalleryAsync(GalleryGameCreateRequestDto dto);
    Task<OperationResult<GalleryGameResponseDto>> UpdateGalleryGameAsync(Guid id, GalleryGameCreateRequestDto dto);
    Task<OperationResult> RemoveFromGalleryAsync(Guid id);
    Task<OperationResult> ApplyPromotionAsync(Guid id, string promotionType, decimal value, DateTime? startDate = null, DateTime? endDate = null);
    Task<OperationResult> RemovePromotionAsync(Guid id);

    // Store operations
    Task<OperationResult<GalleryGameResponseDto>> GetGalleryGameByIdAsync(Guid id);
    Task<OperationResult<PagedResult<GalleryGameResponseDto>>> GetGalleryGamesAsync(PagedRequestDto<GameFilterDto, GameOrderDto> dto);
    Task<OperationResult<IEnumerable<GalleryGameResponseDto>>> GetPromotionalGamesAsync();
    Task<OperationResult<bool>> IsGameAvailableForPurchaseAsync(Guid id);
    Task<OperationResult<decimal>> GetGamePriceAsync(Guid id);
    
    // Player operations
    Task<OperationResult<PagedResult<GalleryGameResponseDto>>> GetAvailableGamesAsync(PagedRequestDto<GameFilterDto, GameOrderDto> dto, Guid playerId);
}