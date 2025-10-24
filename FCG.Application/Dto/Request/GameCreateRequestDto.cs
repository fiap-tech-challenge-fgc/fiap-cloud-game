namespace FCG.Application.Dto.Request;

public class GameCreateRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}