using FCG.Application.Dto.Request;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = RoleConstants.Admin)]
[Tags("Admin")]
[Route("api/admin/cart")]
public class AdminCartController : ControllerBase
{
    private readonly ICartService _cartService;
    public AdminCartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [Swashbuckle.AspNetCore.Annotations.SwaggerOperation()]
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
            return BadRequest(new { errors = result.Errors });

        return NoContent();
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
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    [HttpGet("{playerId}")]
    public async Task<IActionResult> GetCart(Guid playerId)
    {
        var result = await _cartService.GetCartAsync(playerId);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpDelete("{playerId}/clear")]
    public async Task<IActionResult> ClearCart(Guid playerId)
    {
        var result = await _cartService.ClearCartAsync(playerId);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }
}
