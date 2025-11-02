using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/purchases")]
[Authorize(Roles = RoleConstants.Admin)]
public class AdminPurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseAppService;

    public AdminPurchaseController(IPurchaseService purchaseAppService)
    {
        _purchaseAppService = purchaseAppService;
    }

    // 📥 Compra direta de um jogo
    [HttpPost("single")]
    public async Task<IActionResult> RegisterSinglePurchase([FromBody] PurchaseCreateRequestDto dto)
    {
        var result = await _purchaseAppService.RegisterPurchaseAsync(dto);

        if (result.Succeeded)
            return Ok(result.Data);

        return BadRequest(new { result.Errors });
    }

    // 🛒 Compra de todos os itens do carrinho
    [HttpPost("cart/{playerId}")]
    public async Task<IActionResult> RegisterPurchaseFromCart(Guid playerId)
    {
        var result = await _purchaseAppService.RegisterPurchaseAsync(playerId);
        
        if (result.Succeeded)
            return Ok();
        
        return BadRequest(new { result.Errors });
    }

    // 🔍 Listar compras de um jogador
    [HttpGet("player/{playerId}")]
    public async Task<ActionResult<PagedResult<PurchaseResponseDto>>> GetPurchasesByPlayer(Guid playerId, PagedRequestDto<PurchaseFilterDto, PurchaseOrderDto> dto)
    {
        var purchases = await _purchaseAppService.GetPurchasesByPlayerAsync(playerId, dto);
        
        return Ok(purchases.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPurchases([FromQuery] PagedRequestDto<PurchaseFilterDto, PurchaseOrderDto> dto)
    {
        var resultPurchases = await _purchaseAppService.GetAllPurchasesAsync(dto);

        if (!resultPurchases.Succeeded)
            return NotFound(resultPurchases.Errors);

        return Ok(resultPurchases.Data);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetPurchaseStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _purchaseAppService.GetPurchaseStatsAsync(startDate, endDate);

        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }
}
