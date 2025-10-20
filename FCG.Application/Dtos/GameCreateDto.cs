namespace FCG.Application.Dtos;

public class GameCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}