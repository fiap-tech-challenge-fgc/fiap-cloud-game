using FCG.Application.Dtos;
using FCG.Application.Dtos.Response;

namespace FCG.Application.Interfaces;

public interface IPlayerService
{
    Task<OperationResult> CreateAsync(PlayerCreateDto dto);
    Task<OperationResult> UpdateDisplayNameAsync(Guid playerId, string newDisplayName);
    Task<OperationResult> AddGameToLibraryAsync(Guid playerId, Guid gameId);
    Task<PlayerResponseDto?> GetByIdAsync(Guid playerId);
    Task<IEnumerable<PlayerResponseDto>> GetAllAsync();
}