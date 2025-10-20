using FCG.Application.Dtos;
using FCG.Application.Dtos.Response;
using FCG.Application.Interfaces.Service;
using FCG.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FCG.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<(IdentityResult Result, User User)> RegisterUserAsync(UserCreateDto dto, string role)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        LogRegistrationAttempt(dto.Email, result.Succeeded);

        if (!result.Succeeded)
            return (result, user);

        result = await _userManager.AddToRoleAsync(user, role);
        return (result, user);
    }

    public async Task<UserAuthResponseDto> LoginAsync(UserLoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            LogLoginAttempt(dto.Email, false);
            return null!;
        }

        LogLoginAttempt(dto.Email, true);
        return await CreateToken(user);
    }

    public async Task<UserAuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var principal = _jwtService.ValidateToken(dto.Token);
        if (principal == null)
        {
            LogTokenRefreshAttempt("principal inválido", false, "Token inválido");
            return null!;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            LogTokenRefreshAttempt("User id inválido", false, "Token inválido");
            return null!;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !await _jwtService.ValidateRefreshToken(user, dto.RefreshToken))
        {
            LogTokenRefreshAttempt(userId, false, "Refresh token inválido");
            return null!;
        }

        LogTokenRefreshAttempt(userId, true);
        return await CreateToken(user);
    }

    public async Task<UserInfoDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
    {
        var userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            LogGetCurrentUserAttempt(userId!, false);
            return null!;
        }

        LogGetCurrentUserAttempt(userId!, true);
        return GetUserInfo(user);
    }

    public async Task<OperationResult> UpdatePasswordAsync(Guid userId, UserUpdateDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return OperationResult.Failure("Usuário não encontrado.");

        // Verifica se a senha atual está correta
        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
            return OperationResult.Failure("Senha atual incorreta.");

        // Tenta alterar a senha
        var result = await _userManager.ChangePasswordAsync(user, dto.Password, dto.NewPassword);
        if (!result.Succeeded)
            return OperationResult.Failure("Erro ao alterar a senha: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        return OperationResult.Success();
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
            User = GetUserInfo(user)
        };
    }

    private UserInfoDto GetUserInfo(User user)
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
            _logger.LogInformation("Login bem-sucedido para o usuário: {Email}", email);
        else
            _logger.LogWarning("Tentativa de login falhou para o usuário: {Email}", email);
    }

    private void LogTokenRefreshAttempt(string userId, bool success, string message = "")
    {
        if (success)
            _logger.LogInformation("Renovação de token bem-sucedida para o usuário ID: {UserId}", userId);
        else
            _logger.LogWarning("Tentativa de renovação de token falhou para o usuário ID: {UserId}, Motivo: {Motivo}", userId, message);
    }

    private void LogRegistrationAttempt(string email, bool success)
    {
        if (success)
            _logger.LogInformation("Registro bem-sucedido para o usuário: {Email}", email);
        else
            _logger.LogWarning("Tentativa de registro falhou para o usuário: {Email}", email);
    }

    private void LogGetCurrentUserAttempt(string userId, bool success)
    {
        if (success)
            _logger.LogInformation("Recuperação de usuário bem-sucedida para o usuário ID: {UserId}", userId);
        else
            _logger.LogWarning("Tentativa de recuperação de usuário falhou para o usuário ID: {UserId}", userId);
    }
}