using FCG.Api.Dtos;

namespace FCG.Api.Dtos.Response;

public class UserAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfoDto User { get; set; } = new();
}
