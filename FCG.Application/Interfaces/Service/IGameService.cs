using FCG.Application.Dtos;
using FCG.Application.Dtos.Response;

namespace FCG.Application.Interfaces.Service;

public interface IGameService
{
    Task<GameResponseDto> CreateGameAsync(GameCreateDto dto);
    Task<GameResponseDto?> GetGameByIdAsync(Guid id);
    Task<IEnumerable<GameResponseDto>> GetAllGamesAsync();
    Task<OperationResult> DeleteGameAsync(Guid id);
}