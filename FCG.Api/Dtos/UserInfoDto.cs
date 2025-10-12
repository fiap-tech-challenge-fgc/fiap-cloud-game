using FCG.Api.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FCG.Api.Dtos;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
