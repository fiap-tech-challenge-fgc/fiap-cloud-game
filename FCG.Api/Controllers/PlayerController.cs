using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Interfaces;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[Authorize(Roles = RoleConstants.Player)]
[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
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
        // Tenta buscar em várias claims padrão
        var claim = User.FindFirst("sub")?.Value
                       ?? User.FindFirst("nameidentifier")?.Value;

        return Guid.TryParse(claim, out var guid) ? guid : null;
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePlayerPassword([FromBody] UserUpdateRequestDto dto)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var result = await _userService.UpdatePasswordAsync(playerId.Value, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    [HttpGet("available-games")]
    public async Task<IActionResult> GetAvailableGames([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var result = await _galleryService.GetAvailableGamesAsync(dto, playerId.Value);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCartItems()
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var result = await _cartService.GetCartAsync(playerId.Value);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var result = await _libraryService.GetPlayerLibraryAsync(playerId.Value, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
}