namespace FCG.Application.Dtos;

public class PlayerCreateDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}