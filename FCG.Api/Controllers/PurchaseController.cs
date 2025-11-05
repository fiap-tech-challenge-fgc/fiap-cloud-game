using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[Authorize(Roles = RoleConstants.Player)]
[Route("api/[controller]")]
[ApiController]

public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseAppService;

    public PurchaseController(IPurchaseService purchaseAppService)
    {
        _purchaseAppService = purchaseAppService;
    }

    private Guid? GetCurrentUserId()
    {
        // Tenta buscar em várias claims padrão
        var claim = User.FindFirst("sub")?.Value
                       ?? User.FindFirst("nameidentifier")?.Value;

        return Guid.TryParse(claim, out var guid) ? guid : null;
    }

    // 📥 Compra direta de um jogo
    [HttpPost("single")]
    public async Task<IActionResult> RegisterSinglePurchase([FromBody] PurchaseCreateRequestDto dto)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        // se o id for fiferente por enquanto não libera, podemos ver para presentear, pelo displayname algo assim...
        if (playerId != dto.PlayerId)        
            return BadRequest("Ainda não é possivel presentear o jogo para outro jogador.");

        var result = await _purchaseAppService.RegisterPurchaseAsync(dto);

        if (result.Succeeded)
            return Ok(result.Data);

        return BadRequest(new { result.Errors });
    }

    // 🛒 Compra de todos os itens do carrinho
    [HttpPost("cart")]
    public async Task<IActionResult> RegisterPurchaseFromCart()
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var result = await _purchaseAppService.RegisterPurchaseAsync(playerId.Value);

        if (result.Succeeded)
            return Ok();

        return BadRequest(new { result.Errors });
    }

    // 🔍 Listar compras de um jogador
    [HttpGet("player")]
    public async Task<ActionResult<PagedResult<PurchaseResponseDto>>> GetPurchasesByPlayer(PagedRequestDto<PurchaseFilterDto, PurchaseOrderDto> dto)
    {
        var playerId = GetCurrentUserId();

        if (playerId == null)
            return Unauthorized("Não autorizado.");

        var purchases = await _purchaseAppService.GetPurchasesByPlayerAsync(playerId.Value, dto);

        return Ok(purchases.Data);
    }
}
