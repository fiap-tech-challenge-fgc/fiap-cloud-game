using FCG.Api.Controllers.Base;
using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[ApiController]
[Authorize(Roles = "Player")]
[Route("api/[controller]")]
public class LibraryController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILibraryService _libraryService;
    private readonly IGalleryService _galleryService;

    public LibraryController(IAuthService authService,ILibraryService libraryService, IGalleryService galleryService)
    {
        _authService = authService;
        _libraryService = libraryService;
        _galleryService = galleryService;
    }

    [HttpPost("purchase/{gameId}")]
    public async Task<ActionResult<LibraryGameResponseDto>> PurchaseGame(Guid gameId)
    {
        var resultUser = await GetCurrentPlayerId();

        if (!resultUser.Succeeded)
            return BadRequest(new { errors = resultUser.Errors });

        var playerId = resultUser.Data!.Id;

        var canPurchaseResult = await _libraryService.CanPurchaseGameAsync(playerId, gameId);
        if (!canPurchaseResult.Succeeded)
            return BadRequest(new { errors = canPurchaseResult.Errors });

        if (!canPurchaseResult.Data)
            return BadRequest(new { message = "Jogo não disponível para compra ou já existe na biblioteca." });

        var priceResult = await _galleryService.GetGamePriceAsync(gameId);
        if (!priceResult.Succeeded)
            return BadRequest(new { errors = priceResult.Errors });

        var result = await _libraryService.AddToLibraryAsync(playerId, gameId, priceResult.Data);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetLibraryGame), new { gameId }, result.Data);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<LibraryGameResponseDto>>> GetLibrary([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> request)
    {
        var resultUser = await GetCurrentPlayerId();

        if (!resultUser.Succeeded)
            return BadRequest(new { errors = resultUser.Errors });

        var playerId = resultUser.Data!.Id;

        var result = await _libraryService.GetPlayerLibraryAsync(playerId, request);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("{gameId}")]
    public async Task<ActionResult<LibraryGameResponseDto>> GetLibraryGame(Guid gameId)
    {
        var resultUser = await GetCurrentPlayerId();

        if (!resultUser.Succeeded)
            return BadRequest(new { errors = resultUser.Errors });

        var playerId = resultUser.Data!.Id;
        var result = await _libraryService.GetLibraryGameAsync(playerId, gameId);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        if (result.Data == null)
            return NotFound(new { message = "Jogo não encontrado na biblioteca." });
        
        return Ok(result.Data);
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<LibraryGameResponseDto>>> GetRecentPurchases([FromQuery] int count = 5)
    {
        var resultUser = await GetCurrentPlayerId();

        if (!resultUser.Succeeded)
            return BadRequest(new { errors = resultUser.Errors });

        var playerId = resultUser.Data!.Id;
        var result = await _libraryService.GetRecentPurchasesAsync(playerId, count);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<LibraryStatsResponseDto>> GetLibraryStats()
    {
        var resultUser = await GetCurrentPlayerId();

        if (!resultUser.Succeeded)
            return BadRequest(new { errors = resultUser.Errors });

        var playerId = resultUser.Data!.Id;
        var gameCountResult = await _libraryService.GetLibraryGameCountAsync(playerId);

        if (!gameCountResult.Succeeded)
            return BadRequest(new { errors = gameCountResult.Errors });

        var totalSpentResult = await _libraryService.GetTotalSpentAsync(playerId);
        if (!totalSpentResult.Succeeded)
            return BadRequest(new { errors = totalSpentResult.Errors });

        return Ok(new LibraryStatsResponseDto
        {
            GameCount = gameCountResult.Data,
            TotalSpent = totalSpentResult.Data
        });
    }

    [HttpGet("owns/{gameId}")]
    public async Task<ActionResult<bool>> OwnsGame(Guid gameId)
    {
        var resultUser = await GetCurrentPlayerId();

        if (!resultUser.Succeeded)
            return BadRequest(new { errors = resultUser.Errors });

        var playerId = resultUser.Data!.Id;
        var result = await _libraryService.OwnsGameAsync(playerId, gameId);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("can-purchase/{gameId}")]
    public async Task<ActionResult<bool>> CanPurchaseGame(Guid gameId)
    {
        var resultUser = await GetCurrentPlayerId();

        if (!resultUser.Succeeded)
            return BadRequest(new { errors = resultUser.Errors });

        var playerId = resultUser.Data!.Id;

        var result = await _libraryService.CanPurchaseGameAsync(playerId, gameId);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    private async Task<OperationResult<UserInfoResponseDto>> GetCurrentPlayerId()
    {
        return await _authService.GetCurrentUserAsync(User);
    }
}