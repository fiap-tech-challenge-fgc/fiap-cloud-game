namespace FCG.Domain.Entities;
public class Game
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Genre { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public Promotion Promotion { get; private set; } = Promotion.None;

    private Game() { }

    public Game(string name, string genre, string? description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome obrigatório");
        if (string.IsNullOrWhiteSpace(genre)) throw new ArgumentException("Gênero obrigatório");
        if (price < 0) throw new ArgumentException("Preço inválido");

        Id = Guid.NewGuid();
        Name = name;
        Genre = genre;
        Description = description;
        Price = price;
    }

    public decimal PrecoFinal => Promotion.AplicarDesconto(Price);

    public void AplicarPromocao(Promotion promocao)
    {
        Promotion = promocao ?? Promotion.None;
    }

    public string GetDescription()
    {
        return $"{Name} ({Genre}) - {PrecoFinal:C}";
    }
}