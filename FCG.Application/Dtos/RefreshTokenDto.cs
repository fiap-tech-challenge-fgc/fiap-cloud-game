using System.ComponentModel.DataAnnotations;

namespace FCG.Application.Dtos;

public class RefreshTokenDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
