namespace FCG.Application.Dtos.Response;

public class GameResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal FinalPrice { get; set; }
    public string PromotionDescription { get; set; } = string.Empty;
}