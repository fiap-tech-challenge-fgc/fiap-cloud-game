namespace FCG.Application.Dto.Request;

public class GameResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal FinalPrice { get; set; }
    public string PromotionDescription { get; set; } = string.Empty;
    public bool OnSale { get; set; }
    public string TypePromotion { get; set; } = string.Empty;
    public decimal PromotionValue { get; set; }
}