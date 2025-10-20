using System.ComponentModel.DataAnnotations;

namespace FCG.Application.Dtos;

public class UserUpdateDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime Birthday { get; set; }

    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [DataType(DataType.Password)]
    public required string NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Senhas não conferem Nova senha confimação da nova senha")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}