using System.ComponentModel.DataAnnotations;

namespace FCG.Domain.Entities;

public class Game
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do jogo é obrigatório.")]
    [StringLength(25, ErrorMessage = "O nome não pode ter mais de 50 caracteres.")]
    public string? Name { get; set; }

    [StringLength(150, ErrorMessage = "A descrição não pode ter mais de 100 caracteres.")]
    public string? Description { get; set; }

    [Range(minimum: 0.00, maximum: 99999.99, ErrorMessage = "O valor deve constar ente {0} e {1}")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "O gênero do jogo é obrigatório.")]
    [StringLength(50, ErrorMessage = "O gênero não pode ter mais de 30 caracteres.")]
    
    public string? Genre { get; set; }

    public Game()
    {

    }

    public Game(string name, string genre, string? description, decimal price)
    {
        Name = name;
        Genre = genre;
        Description = description;
        Price = price;
    }

    public string GetDescription()
    {
        return $"{Name ?? "-"} ({Genre ?? "-"}) - {Price:C}";
    }
}
