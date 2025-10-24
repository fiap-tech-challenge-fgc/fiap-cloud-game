using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Interfaces.Service;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-player")]
    public async Task<IActionResult> Register([FromBody] UserCreateRequestDto dto)
    {
        var registerResult = await _authService.RegisterUserAsync(dto, RoleConstants.Admin);

        if (!registerResult.Succeeded)
            return BadRequest(new { errors = registerResult.Errors });

        var loginResult = await _authService.LoginAsync(new UserLoginRequestDto
        {
            Email = dto.Email,
            Password = dto.Password
        });

        if (!loginResult.Succeeded)
            return BadRequest(new { errors = loginResult.Errors });

        return Ok(loginResult.Data);

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _authService.LoginAsync(dto);

        if (token == null)
            return Unauthorized(new { message = "Credenciais inválidas" });

        return Ok(token);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _authService.RefreshTokenAsync(dto);

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