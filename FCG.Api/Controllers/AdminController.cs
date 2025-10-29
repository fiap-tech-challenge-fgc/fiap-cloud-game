using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Interfaces;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using FCG.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = RoleConstants.Admin)]
public class AdminController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IGameService _gameService;
    private readonly IGalleryService _galleryService;
    private readonly IPurchaseService _purchaseAppService;

    public AdminController(
        IAuthService authService,
        IUserService userService,
        IGameService gameService,
        IGalleryService galleryService,
        IPurchaseService purchaseAppService)
    {
        _authService = authService;
        _userService = userService;
        _gameService = gameService;
        _galleryService = galleryService;
        _purchaseAppService = purchaseAppService;
    }

    // 👤 Exibe um usuário
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (!result.Succeeded)
            return NotFound(new { message = result.Errors });


        return Ok(result.Data);
    }

    // 👤 Lista todos usuários
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] PagedRequestDto<UserFilterDto, UserOrderDto> dto)
    {
        var users = await _userService.GetAllAsync(dto);
        return Ok(users.Data);
    }

    // 🔐 Cadastrar novo admin
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] UserCreateRequestDto dto)
    {
        var result = await _authService.RegisterUserAsync(dto, RoleConstants.Admin);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        // ✅ Retorna apenas os dados do usuário criado
        return CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, result.Data);
    }

    // 👤 Listar todos os admins
    [HttpGet("admins")]
    public async Task<IActionResult> GetAllAdmins([FromQuery] PagedRequestDto<UserFilterDto,UserOrderDto> dto)
    {
        var result = await _userService.GetUsersByRoleAsync(Roles.Admin, dto);
        return Ok(result.Data);
    }

    // 👤 Atualizar dados de um admin
    [HttpPut("admins/{id}")]
    public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] UserUpdateRequestDto dto)
    {
        var result = await _userService.UpdateUserAsync(id, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    // 👤 Remover um admin
    [HttpDelete("admins/{id}")]
    public async Task<IActionResult> DeleteAdmin(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    // 👤 Listar todos os players
    [HttpGet("players")]
    public async Task<IActionResult> GetAllPlayers([FromQuery] PagedRequestDto<UserFilterDto, UserOrderDto> dto)
    {
        var result = await _userService.GetUsersByRoleAsync(Roles.Player, dto);
        return Ok(result.Data);
    }

    // 👤 Atualizar dados de um player
    [HttpPut("players/{id}")]
    public async Task<IActionResult> UpdatePlayer(Guid id, [FromBody] UserUpdateRequestDto dto)
    {
        var result = await _userService.UpdateUserAsync(id, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    // 👤 Remover um player
    [HttpDelete("players/{id}")]
    public async Task<IActionResult> DeletePlayer(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    // 🕹️ Game Management
    [HttpPost("games")]
    public async Task<IActionResult> CreateGame([FromBody] GameCreateRequestDto dto)
    {
        var result = await _gameService.CreateGameAsync(dto);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetGameById), new { id = result.Data!.Id }, result.Data);
    }

    // 🕹️ Game Management
    [HttpGet("games/{id}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var result = await _gameService.GetGameByIdAsync(id);
        if (!result.Succeeded)
            return NotFound(new { message = result.Errors });

        return Ok(result.Data);
    }

    // 🕹️ Game Management
    [HttpGet("games")]
    public async Task<IActionResult> GetAllGames([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        var result = await _gameService.GetAllGamesAsync(dto);
        return Ok(result.Data);
    }

    // 🕹️ Game Management
    [HttpDelete("games/{id}")]
    public async Task<IActionResult> DeleteGame(Guid id)
    {
        var result = await _gameService.DeleteGameAsync(id);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }

    // 🎮 Gallery Management
    [HttpPost("games/gallery")]
    public async Task<ActionResult<GalleryGameResponseDto>> AddToGallery(GalleryGameCreateRequestDto request)
    {
        var result = await _galleryService.AddToGalleryAsync(request);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetGalleryGame), new { id = result.Data.Id }, result.Data);
    }

    [HttpGet("games/gallery/{id}")]
    public async Task<ActionResult<GalleryGameResponseDto>> GetGalleryGame(Guid id)
    {
        var result = await _galleryService.GetGalleryGameByIdAsync(id);
        if (!result.Succeeded)
            return NotFound(result.Errors);
        
        return Ok(result.Data);
    }

    [HttpPut("games/gallery/{id}")]
    public async Task<ActionResult<GalleryGameResponseDto>> UpdateGalleryGame(Guid id, GalleryGameCreateRequestDto request)
    {
        var result = await _galleryService.UpdateGalleryGameAsync(id, request);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpDelete("games/gallery/{id}")]
    public async Task<ActionResult> RemoveFromGallery(Guid id)
    {
        var result = await _galleryService.RemoveFromGalleryAsync(id);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

    [HttpPost("games/gallery/{id}/promotion")]
    public async Task<ActionResult> ApplyPromotion(Guid id, [FromBody] GalleryPromotionRequestDto request)
    {
        var result = await _galleryService.ApplyPromotionAsync(
            id,
            request.PromotionType,
            request.PromotionValue,
            request.PromotionStartDate,
            request.PromotionEndDate);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

    [HttpDelete("games/gallery/{id}/promotion")]
    public async Task<ActionResult> RemovePromotion(Guid id)
    {
        var result = await _galleryService.RemovePromotionAsync(id);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

    // 📃 Purchase Management
    [HttpGet("purchases")]
    public async Task<IActionResult> GetAllPurchases([FromQuery] PagedRequestDto<PurchaseFilterDto, PurchaseOrderDto> dto)
    {
        var resultPurchases = await _purchaseAppService.GetAllPurchasesAsync(dto);

        if (!resultPurchases.Succeeded)
            return NotFound(resultPurchases.Errors);

        return Ok(resultPurchases.Data);
    }

    [HttpGet("purchases/stats")]
    public async Task<IActionResult> GetPurchaseStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _purchaseAppService.GetPurchaseStatsAsync(startDate, endDate);
        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }
}