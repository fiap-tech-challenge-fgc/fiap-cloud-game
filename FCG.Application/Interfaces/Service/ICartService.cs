using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;

namespace FCG.Application.Interfaces.Service;

public interface ICartService
{
    Task<OperationResult> AddItemAsync(CartItemRequestDto request);
    Task<OperationResult> RemoveItemAsync(CartItemRequestDto request);
    Task<OperationResult<CartResponseDto>> GetCartByPlayerIdAsync(Guid playerId);
    Task<OperationResult<CartResponseDto>> GetCartByUserIdAsync(Guid userId);
    Task<OperationResult> ClearCartAsync(Guid playerId);
}