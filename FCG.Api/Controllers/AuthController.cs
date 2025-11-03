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

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-player")]
    public async Task<IActionResult> Register([FromBody] UserCreateRequestDto dto)
    {
        var result = await _authService.RegisterUserAsync(dto, RoleConstants.Player);

        if (!result.Succeeded)
            return BadRequest(new { data = (UserAuthResponseDto?)null, succeeded = false, errors = result.Errors });

        var loginResult = await _authService.LoginAsync(new UserLoginRequestDto
        {
            Email = dto.Email,
            Password = dto.Password
        });

        if (!loginResult.Succeeded)
            return BadRequest(new { data = (UserAuthResponseDto?)null, succeeded = false, errors = loginResult.Errors });

        return Ok(new { data = loginResult.Data, succeeded = true, errors = new List<string>() });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { data = (UserAuthResponseDto?)null, succeeded = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        var result = await _authService.LoginAsync(dto);

        if (!result.Succeeded)
            return Unauthorized(new { data = (UserAuthResponseDto?)null, succeeded = false, errors = result.Errors });

        return Ok(new { data = result.Data, succeeded = true, errors = new List<string>() });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { data = (UserAuthResponseDto?)null, succeeded = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        var result = await _authService.RefreshTokenAsync(dto);

        if (!result.Succeeded)
            return Unauthorized(new { data = (UserAuthResponseDto?)null, succeeded = false, errors = result.Errors });

        return Ok(new { data = result.Data, succeeded = true, errors = new List<string>() });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _authService.GetCurrentUserAsync(User);

        if (!result.Succeeded)
            return NotFound(new { data = (UserInfoResponseDto?)null, succeeded = false, errors = new[] { "Usuário não encontrado" } });

        return Ok(new { data = result.Data, succeeded = true, errors = new List<string>() });
    }
}