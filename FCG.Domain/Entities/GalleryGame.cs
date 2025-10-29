using FCG.Domain.ValuesObjects;

namespace FCG.Domain.Entities;

public class GalleryGame : Game
{
    public decimal Price { get; private set; }
    public Promotion Promotion { get; private set; } = Promotion.None;
    private GalleryGame() : base() { }
    public GalleryGame(string ean, string name, string genre, string? description, decimal price) 
        : base(ean, name, genre, description)
    {

        if (price < 0) 
            throw new ArgumentException("Preço inválido");

        Id = Guid.NewGuid();
        Price = price;
    }

    public decimal FinalPrice => Math.Max(0, Promotion.ApplyDiscount(Price));

    public void ApplyPromotion(Promotion promotion)
    {
        Promotion = promotion ?? Promotion.None;
    }

    public override string GetDescription()
    {
        return $"{base.GetDescription()} - {FinalPrice:C}";
    }
}