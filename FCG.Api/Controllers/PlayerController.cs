using FCG.Application.Dtos;
using FCG.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly IUserService _userService;
    private readonly IPlayerService _playerService;    

    public PlayerController(
        ILogger<PlayerController> logger,
        IUserService userService,
        IPlayerService playerService)
    {
        _logger = logger;
        _userService = userService;
        _playerService = playerService;
    }

    private Guid GetAuthorizedUserGuid()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Usuário não autenticado corretamente.");

        return Guid.Parse(userId);
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePlayerPassword([FromBody] UserUpdateDto dto)
    {
        var id = GetAuthorizedUserGuid();
        var result = await _userService.UpdatePasswordAsync(id, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    [HttpGet("available-games")]
    public async Task<IActionResult> GetAvailableGames(
        [FromQuery] string? orderBy = null,
        [FromQuery] bool excludeOwned = false)
    {
        var id = GetAuthorizedUserGuid();
        var result = await _playerService.GetAvailableGamesAsync(orderBy, excludeOwned, id);
        return Ok(result);
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCartItems()
    {
        var id = GetAuthorizedUserGuid();
        var result = await _playerService.GetCartItemsAsync(id);
        return Ok(result);
    }

    [HttpGet("purchases")]
    public async Task<IActionResult> GetPurchasedGames()
    {
        var id = GetAuthorizedUserGuid();
        var result = await _playerService.GetPurchasedGamesAsync(id);
        return Ok(result);
    }
}