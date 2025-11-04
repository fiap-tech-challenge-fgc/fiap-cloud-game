using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Interfaces;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using FCG.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.Api.Controllers;

[Authorize(Roles = RoleConstants.Player)]
[ApiController]
[Route("api/[controller]")]
public class PlayerController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly IPlayerService _playerService;
    private readonly IGalleryService _galleryService;
    private readonly ICartService _cartService;
    private readonly ILibraryService _libraryService;

    public PlayerController(
        IUserService userService,
        IPlayerService playerService,
        IGalleryService galleryService,
        ICartService cartService,
        ILibraryService libraryService)
    {
        _userService = userService;
        _playerService = playerService;
        _galleryService = galleryService;
        _cartService = cartService;
        _libraryService = libraryService;
    }

    private Guid? GetCurrentUserId()
    {
        // Usa ClaimTypes.NameIdentifier que é mapeado corretamente pelo JWT middleware
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(claim, out var guid) ? guid : null;
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePlayerPassword([FromBody] UserUpdateRequestDto dto)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return UnauthorizedResult<object>("Não autorizado.");

        var result = await _userService.UpdatePasswordAsync(playerId.Value, dto);
        
        return ProcessResult(result);
    }

    [HttpGet("available-games")]
    public async Task<IActionResult> GetAvailableGames([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return UnauthorizedResult<object>("Não autorizado.");

        var result = await _galleryService.GetAvailableGamesAsync(dto, playerId.Value);

        return ProcessResult(result);
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCartItems()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
            return UnauthorizedResult<object>("Não autorizado.");

        var result = await _cartService.GetCartByUserIdAsync(userId.Value);

        return ProcessResult(result);
    }

    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return UnauthorizedResult<object>("Não autorizado.");

        var result = await _libraryService.GetPlayerLibraryAsync(playerId.Value, dto);
        
        return ProcessResult(result);
    }
}