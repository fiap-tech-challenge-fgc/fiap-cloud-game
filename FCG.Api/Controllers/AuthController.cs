using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Realiza o login do usuário
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT e informações do usuário</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // TODO: Injetar e usar o serviço de autenticação da camada Application
            // Exemplo: var result = await _authService.LoginAsync(request);
            
            _logger.LogInformation("Tentativa de login para o usuário: {Email}", request.Email);

            // Implementação temporária - substituir pela chamada ao serviço real
            return Unauthorized(new { message = "Credenciais inválidas" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login para o usuário: {Email}", request.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Erro ao processar login" });
        }
    }

    /// <summary>
    /// Renova o token JWT usando o refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Novo token JWT</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // TODO: Implementar lógica de refresh token
            _logger.LogInformation("Tentativa de renovação de token");
            
            return Unauthorized(new { message = "Token inválido" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Erro ao renovar token" });
        }
    }

    /// <summary>
    /// Realiza o logout do usuário
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // TODO: Implementar lógica de logout (invalidar refresh token)
            _logger.LogInformation("Usuário realizou logout");
            
            return Ok(new { message = "Logout realizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar logout");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Erro ao processar logout" });
        }
    }
}

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string AccessToken, 
    string RefreshToken, 
    DateTime ExpiresAt, 
    string UserName, 
    string Email,
    IEnumerable<string> Roles);

public record RefreshTokenRequest(string RefreshToken);