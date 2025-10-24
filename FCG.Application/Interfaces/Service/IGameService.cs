using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;

public interface IGameService
{
    Task<OperationResult<GameResponseDto>> CreateGameAsync(GameCreateRequestDto dto);
    Task<GameResponseDto?> GetGameByIdAsync(Guid id);
    Task<PagedResult<GameResponseDto>> GetAllGamesAsync(PagedRequestDto<GameFilterDto, GameOrderDto> pagedRequestDto);
    Task<OperationResult> DeleteGameAsync(Guid id);
}