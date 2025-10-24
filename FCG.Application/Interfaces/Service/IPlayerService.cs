using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;

namespace FCG.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<OperationResult> CreateAsync(PlayerCreateDto dto);
        Task<OperationResult> UpdateDisplayNameAsync(Guid playerId, string newDisplayName);
        Task<OperationResult> AddGameToLibraryAsync(Guid playerId, Guid gameId);
        Task<IEnumerable<PlayerResponseDto>> GetAllAsync();
        Task<PlayerWithUserResponseDto?> GetByIdAsync(Guid playerId);
        Task<IEnumerable<GameResponseDto>> GetPurchasedGamesAsync(Guid playerId);
        Task<IEnumerable<GameResponseDto>> GetCartItemsAsync(Guid playerId);
        Task<IEnumerable<GameResponseDto>> GetAvailableGamesAsync(string? orderBy, bool excludeOwned, Guid? playerId);
    }
}