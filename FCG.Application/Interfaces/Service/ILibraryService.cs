using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;

namespace FCG.Application.Interfaces.Service;

public interface ILibraryService
{
    // Library management
    Task<OperationResult<LibraryGameResponseDto>> AddToLibraryAsync(Guid playerId, Guid galleryGameId, decimal purchasePrice);
    Task<OperationResult<LibraryGameResponseDto>> GetLibraryGameAsync(Guid playerId, Guid gameId);
    Task<OperationResult<PagedResult<LibraryGameResponseDto>>> GetPlayerLibraryAsync(Guid playerId, PagedRequestDto<GameFilterDto, GameOrderDto> pagedRequestDto);
    
    // Ownership verification
    Task<OperationResult<bool>> OwnsGameAsync(Guid playerId, Guid gameId);
    Task<OperationResult<bool>> CanPurchaseGameAsync(Guid playerId, Guid gameId);
    
    // Library statistics
    Task<OperationResult<int>> GetLibraryGameCountAsync(Guid playerId);
    Task<OperationResult<decimal>> GetTotalSpentAsync(Guid playerId);
    Task<OperationResult<IEnumerable<LibraryGameResponseDto>>> GetRecentPurchasesAsync(Guid playerId, int count = 5);
}