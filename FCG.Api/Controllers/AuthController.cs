using FCG.Api.Dtos;
using FCG.Api.Dtos.Response;
using FCG.Infrastructure.Identity;
using FCG.Infrastructure.Interfaces;
using FCG.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FCG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IJwtService _jwtService;
    private readonly UserManager<AppUserIdentity> _userManager;
    private readonly SignInManager<AppUserIdentity> _signInManager;

    public AuthController(
        ILogger<AuthController> logger,
        IJwtService jwtService,
        UserManager<AppUserIdentity> userManager,
        SignInManager<AppUserIdentity> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _jwtService = jwtService;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new AppUserIdentity
        {
            UserName = createUserDto.Email,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        // Adicionar role padrão se quiser
        // await _userManager.AddToRoleAsync(user, "User");

        var token = await _jwtService.GenerateToken(user);
        var refreshToken = await _jwtService.GenerateRefreshToken(user);

        _logger.LogInformation("Novo usuário cadastrado: {DisplayName}, {Email}", createUserDto.DisplayName, createUserDto.Email);

        return Ok(new UserAuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserInfoDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return Unauthorized(new { message = "Credenciais inválidas" });

        var token = await _jwtService.GenerateToken(user);
        var refreshToken = await _jwtService.GenerateRefreshToken(user);

        _logger.LogInformation("Tentativa de login para o usuário: {Email}", loginDto.Email);

        return Ok(new UserAuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var principal = _jwtService.ValidateToken(refreshTokenDto.Token);
        if (principal == null)
            return Unauthorized(new { message = "Token inválido" });

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized(new { message = "Token inválido" });

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !await _jwtService.ValidateRefreshToken(user, refreshTokenDto.RefreshToken))
            return Unauthorized(new { message = "Refresh token inválido" });

        var newToken = await _jwtService.GenerateToken(user);
        var newRefreshToken = await _jwtService.GenerateRefreshToken(user);

        _logger.LogInformation("Tentativa de renovação de token");

        return Ok(new UserAuthResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName
            }
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        _logger.LogInformation("Usuário logado {DisplayName}", user.DisplayName);

        return Ok(new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName
        });
    }
}