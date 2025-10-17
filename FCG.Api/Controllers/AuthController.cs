using FCG.Api.Dtos;
using FCG.Api.Dtos.Response;
using FCG.Domain.Entities;
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
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(
        ILogger<AuthController> logger,
        IJwtService jwtService,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
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

        var user = new User
        {
            UserName = createUserDto.Email,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);

        this.LogRegistrationAttempt(createUserDto.Email, result.Succeeded);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        // Adicionar role padrão se quiser
        // await _userManager.AddToRoleAsync(user, "User");

        var token = await CreateToken(user);
        return Ok(token);
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
        {
            this.LogLoginAttempt(loginDto.Email, false);
            return Unauthorized(new { message = "Credenciais inválidas" });
        }

        this.LogLoginAttempt(loginDto.Email, false);

        var token = await CreateToken(user);
        return Ok(token);
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
        {
            this.LogTokenRefreshAttempt("principal inválido", false, "Token inválido");
            return Unauthorized(new { message = "Token inválido" });
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            this.LogTokenRefreshAttempt("User id inválido", false, "Token inválido");
            return Unauthorized(new { message = "Token inválido" });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !await _jwtService.ValidateRefreshToken(user, refreshTokenDto.RefreshToken))
        {
            this.LogTokenRefreshAttempt(userId, false, "Refresh token inválido");
            return Unauthorized(new { message = "Refresh token inválido" });
        }

        this.LogTokenRefreshAttempt(userId, false);

        var token = await CreateToken(user);
        return Ok(token);
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

        return Ok(GetUsetInfo(user));
    }

    private async Task<UserAuthResponseDto> CreateToken(User user)
    {
        var token = await _jwtService.GenerateToken(user);
        var refreshToken = await _jwtService.GenerateRefreshToken(user);

        return new UserAuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = GetUsetInfo(user)
        };
    }

    private UserInfoDto GetUsetInfo(User user)
    {
        return new UserInfoDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!
        };
    }

    private void LogLoginAttempt(string email, bool success)
    {
        if (success)
        {
            _logger.LogInformation("Login bem-sucedido para o usuário: {Email}", email);
        }
        else
        {
            _logger.LogWarning("Tentativa de login falhou para o usuário: {Email}", email);
        }
    }

    private void LogTokenRefreshAttempt(string userId, bool success, string message = "")
    {
        if (success)
        {
            _logger.LogInformation("Renovação de token bem-sucedida para o usuário ID: {UserId}", userId);
        }
        else
        {
            _logger.LogWarning("Tentativa de renovação de token falhou para o usuário ID: {UserId}, Motivo: {Motivo}", userId, message);
        }
    }

    private void LogRegistrationAttempt(string email, bool success)
    {
        if (success)
        {
            _logger.LogInformation("Registro bem-sucedido para o usuário: {Email}", email);
        }
        else
        {
            _logger.LogWarning("Tentativa de registro falhou para o usuário: {Email}", email);
        }
    }

    private void LogGetCurrentUserAttempt(string userId, bool success)
    {
        if (success)
        {
            _logger.LogInformation("Recuperação de usuário bem-sucedida para o usuário ID: {UserId}", userId);
        }
        else
        {
            _logger.LogWarning("Tentativa de recuperação de usuário falhou para o usuário ID: {UserId}", userId);
        }
    }
}