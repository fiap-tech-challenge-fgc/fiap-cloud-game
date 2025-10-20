using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FCG.Application.Dtos;

public class UserLoginDto
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;
}
