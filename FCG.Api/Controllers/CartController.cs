using FCG.Api.Controllers.Base;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.API.Controllers;

[ApiController]
[Authorize(Roles = RoleConstants.Player)]
[Route("api/cart")]
public class CartController : ApiControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("add/{gameId}")]
    public async Task<IActionResult> AddItem(Guid gameId)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
            return UnauthorizedResult<object>("Não autorizado.");

        var request = new CartItemRequestDto
        {
            UserId = userId.Value,
            GameId = gameId
        };

        var result = await _cartService.AddItemAsync(request);

        return ProcessResult(result);
    }

    [HttpDelete("remove/{gameId}")]
    public async Task<IActionResult> RemoveItem(Guid gameId)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return UnauthorizedResult<object>("Não autorizado.");

        var request = new CartItemRequestDto
        {
            PlayerId = playerId.Value,
            GameId = gameId
        };

        var result = await _cartService.RemoveItemAsync(request);

        return ProcessResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return UnauthorizedResult<CartResponseDto>("Não autorizado.");

        var result = await _cartService.GetCartByPlayerIdAsync(playerId.Value);

        return ProcessResult(result);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return UnauthorizedResult<object>("Não autorizado.");

        var result = await _cartService.ClearCartAsync(playerId.Value);

        return ProcessResult(result);
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(claim, out var guid) ? guid : null;
    }
}
