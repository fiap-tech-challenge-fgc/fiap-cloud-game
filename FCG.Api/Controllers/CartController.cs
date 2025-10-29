using FCG.Application.Dto.Request;
using FCG.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("{playerId}/add/{gameId}")]
    public async Task<IActionResult> AddItem(Guid playerId, Guid gameId)
    {
        var request = new CartItemRequestDto
        {
            PlayerId = playerId,
            GameId = gameId
        };

        var result = await _cartService.AddItemAsync(request);

        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok();
    }

    [HttpDelete("{playerId}/remove/{gameId}")]
    public async Task<IActionResult> RemoveItem(Guid playerId, Guid gameId)
    {
        var request = new CartItemRequestDto
        {
            PlayerId = playerId,
            GameId = gameId
        };
        
        var result = await _cartService.RemoveItemAsync(request);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok();
    }

    [HttpGet("{playerId}")]
    public async Task<IActionResult> GetCart(Guid playerId)
    {
        var result = await _cartService.GetCartAsync(playerId);

        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok();
    }

    [HttpDelete("{playerId}/clear")]
    public async Task<IActionResult> ClearCart(Guid playerId)
    {
        var result = await _cartService.ClearCartAsync(playerId);

        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok();
    }
}
