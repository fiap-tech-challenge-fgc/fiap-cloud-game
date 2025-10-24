namespace FCG.Application.Dto.Response;

public class PlayerWithUserResponseDto
{
    public Guid PlayerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Games { get; set; } = new();
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}