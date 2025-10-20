using FCG.Application.Dtos;
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
public class AdminAuthController : ControllerBase
{
    private readonly ILogger<AdminAuthController> _logger;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IGameService _gameService;

    public AdminAuthController(
        ILogger<AdminAuthController> logger,
        IAuthService authService,
        IUserService userService,
        IGameService gameService)
    {
        _logger = logger;
        _authService = authService;
        _userService = userService;
        _gameService = gameService;
    }

    // 🔐 Cadastrar novo admin
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] UserCreateDto dto)
    {
        var (result, user) = await _authService.RegisterUserAsync(dto, RoleConstants.Admin);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        var token = await _authService.LoginAsync(new UserLoginDto
        {
            Email = dto.Email,
            Password = dto.Password
        });

        return Ok(token);
    }

    // 👤 Listar todos os admins
    [HttpGet("admins")]
    public async Task<IActionResult> GetAllAdmins()
    {
        var admins = await _userService.GetUsersByRoleAsync(Roles.Admin);
        return Ok(admins);
    }

    // 👤 Atualizar dados de um admin
    [HttpPut("admins/{id}")]
    public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] UserUpdateDto dto)
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
    public async Task<IActionResult> GetAllPlayers()
    {
        var players = await _userService.GetUsersByRoleAsync(Roles.Player);
        return Ok(players);
    }

    // 👤 Atualizar dados de um player
    [HttpPut("players/{id}")]
    public async Task<IActionResult> UpdatePlayer(Guid id, [FromBody] UserUpdateDto dto)
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
    public async Task<IActionResult> CreateGame([FromBody] GameCreateDto dto)
    {
        var game = await _gameService.CreateGameAsync(dto);
        return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, game);
    }

    // 🕹️ Buscar game por ID
    [HttpGet("games/{id}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await _gameService.GetGameByIdAsync(id);
        if (game == null)
            return NotFound();

        return Ok(game);
    }

    // 🕹️ Listar todos os games
    [HttpGet("games")]
    public async Task<IActionResult> GetAllGames()
    {
        var games = await _gameService.GetAllGamesAsync();
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