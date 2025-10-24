using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
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

    public AdminController(
        IAuthService authService,
        IUserService userService,
        IGameService gameService)
    {
        _authService = authService;
        _userService = userService;
        _gameService = gameService;
    }

    // 👤 Exibe um usuário
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        return Ok(user);
    }

    // 👤 Lista todos usuários
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] PagedRequestDto<UserFilterDto, UserOrderDto> dto)
    {
        var users = await _userService.GetAllAsync(dto);
        return Ok(users);
    }

    // 🔐 Cadastrar novo admin
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] UserCreateRequestDto dto)
    {
        var registerResult = await _authService.RegisterUserAsync(dto, RoleConstants.Admin);

        if (!registerResult.Succeeded)
            return BadRequest(new { errors = registerResult.Errors });

        // ✅ Retorna apenas os dados do usuário criado
        return CreatedAtAction(nameof(GetUserById), new { id = registerResult.Data!.Id }, registerResult.Data);

    }

    // 👤 Listar todos os admins
    [HttpGet("admins")]
    public async Task<IActionResult> GetAllAdmins([FromQuery] PagedRequestDto<UserFilterDto,UserOrderDto> dto)
    {
        var admins = await _userService.GetUsersByRoleAsync(Roles.Admin, dto);
        return Ok(admins);
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
        var players = await _userService.GetUsersByRoleAsync(Roles.Player, dto);
        return Ok(players);
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

    // 🕹️ Criar novo game
    [HttpPost("games")]
    public async Task<IActionResult> CreateGame([FromBody] GameCreateRequestDto dto)
    {
        var result = await _gameService.CreateGameAsync(dto);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetGameById), new { id = result.Data!.Id }, result.Data);
    }

    // 🕹️ Buscar game por ID
    [HttpGet("games/{id}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await _gameService.GetGameByIdAsync(id);
        if (game == null)
            return NotFound(new { message = "Jogo não encontrado" });

        return Ok(game);
    }

    // 🕹️ Listar todos os games com paginação
    [HttpGet("games")]
    public async Task<IActionResult> GetAllGames([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        var games = await _gameService.GetAllGamesAsync(dto);
        return Ok(games);
    }

    // 🕹️ Remover um game
    [HttpDelete("games/{id}")]
    public async Task<IActionResult> DeleteGame(Guid id)
    {
        var result = await _gameService.DeleteGameAsync(id);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }
}