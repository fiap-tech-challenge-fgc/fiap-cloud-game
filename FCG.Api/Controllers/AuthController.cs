using FCG.Application.Dtos;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FCG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register-player")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto createUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (result, user) = await _authService.RegisterUserAsync(createUserDto, RoleConstants.Player);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        var token = await _authService.LoginAsync(new UserLoginDto
        {
            Email = createUserDto.Email,
            Password = createUserDto.Password
        });

        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _authService.LoginAsync(loginDto);

        if (token == null)
            return Unauthorized(new { message = "Credenciais inválidas" });

        return Ok(token);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _authService.RefreshTokenAsync(refreshTokenDto);

        if (token == null)
            return Unauthorized(new { message = "Token inválido ou expirado" });

        return Ok(token);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userInfo = await _authService.GetCurrentUserAsync(User);

        if (userInfo == null)
            return NotFound(new { message = "Usuário não encontrado" });

        return Ok(userInfo);
    }
}