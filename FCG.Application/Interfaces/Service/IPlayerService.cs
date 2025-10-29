using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;

namespace FCG.Application.Interfaces.Service;

public interface IPlayerService
{
    Task<OperationResult<PlayerWithUserResponseDto>> GetByIdAsync(Guid playerId);
    Task<OperationResult> CreateAsync(PlayerCreateDto dto);
    Task<OperationResult> UpdateDisplayNameAsync(Guid playerId, string newDisplayName);
    Task<OperationResult<IEnumerable<PlayerResponseDto>>> GetAllAsync();
    Task<bool> ExistsAsync(Guid playerId);
    Task<bool> ExistsByUserIdAsync(Guid userId);
}