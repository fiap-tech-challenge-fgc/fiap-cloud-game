using FCG.Application.Dto.Request;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.API.Controllers;

[ApiController]
[Authorize(Roles = RoleConstants.Player)]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("add/{gameId}")]
    public async Task<IActionResult> AddItem(Guid gameId)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var request = new CartItemRequestDto
        {
            PlayerId = playerId.Value,
            GameId = gameId
        };

        var result = await _cartService.AddItemAsync(request);

        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok();
    }

    [HttpDelete("remove/{gameId}")]
    public async Task<IActionResult> RemoveItem(Guid gameId)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var request = new CartItemRequestDto
        {
            PlayerId = playerId.Value,
            GameId = gameId
        };

        var result = await _cartService.RemoveItemAsync(request);

        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var result = await _cartService.GetCartAsync(playerId.Value);

        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok(result.Data);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var result = await _cartService.ClearCartAsync(playerId.Value);

        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok();
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(claim, out var guid) ? guid : null;
    }
}
