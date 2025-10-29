using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Interfaces;
using FCG.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[Authorize]
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

    private Guid GetAuthorizedUserGuid()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Usuário não autenticado corretamente.");

        return Guid.Parse(userId);
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePlayerPassword([FromBody] UserUpdateRequestDto dto)
    {
        var id = GetAuthorizedUserGuid();
        var result = await _userService.UpdatePasswordAsync(id, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    [HttpGet("available-games")]
    public async Task<IActionResult> GetAvailableGames([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        var id = GetAuthorizedUserGuid();
        var result = await _galleryService.GetAvailableGamesAsync(dto, id);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCartItems()
    {
        var id = GetAuthorizedUserGuid();
        var result = await _cartService.GetCartAsync(id);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        var id = GetAuthorizedUserGuid();
        var result = await _libraryService.GetPlayerLibraryAsync(id, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
}