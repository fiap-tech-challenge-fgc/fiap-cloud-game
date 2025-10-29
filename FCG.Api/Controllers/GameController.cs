using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameResponseDto>> GetGame(Guid id)
    {
        var result = await _gameService.GetGameByIdAsync(id);

        if (!result.Succeeded)
            return NotFound(new { result.Errors });

        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GameResponseDto>>> GetGames([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> request)
    {
        var result = await _gameService.GetAllGamesAsync(request);

        if (!result.Succeeded)
            return NotFound(new { result.Errors });

        return Ok(result.Data);
    }
}