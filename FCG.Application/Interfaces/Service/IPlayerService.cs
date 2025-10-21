using FCG.Application.Dtos;
using FCG.Application.Dtos.Response;

namespace FCG.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<OperationResult> CreateAsync(PlayerCreateDto dto);
        Task<OperationResult> UpdateDisplayNameAsync(Guid playerId, string newDisplayName);
        Task<OperationResult> AddGameToLibraryAsync(Guid playerId, Guid gameId);
        Task<IEnumerable<PlayerResponseDto>> GetAllAsync();
        Task<PlayerWithUserDto?> GetByIdAsync(Guid playerId);
        Task<IEnumerable<GameDto>> GetPurchasedGamesAsync(Guid playerId);
        Task<IEnumerable<GameDto>> GetCartItemsAsync(Guid playerId);
        Task<IEnumerable<GameListDto>> GetAvailableGamesAsync(string? orderBy, bool excludeOwned, Guid? playerId);
    }
}