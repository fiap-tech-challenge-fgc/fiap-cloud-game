using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FCG.Domain.Entities;

[Index(nameof(EAN), IsUnique = true)]
public class Game
{
    public Guid Id { get; private set; }

    [Required]
    public string EAN { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string Genre { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    protected Game() { }

    public Game(string ean, string name, string genre, string? description)
    {
        if (string.IsNullOrWhiteSpace(ean)) throw new ArgumentException("EAN obrigatório");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome obrigatório");
        if (string.IsNullOrWhiteSpace(genre)) throw new ArgumentException("Gênero obrigatório");

        Id = Guid.NewGuid();
        EAN = ean;
        Name = name;
        Genre = genre;
        Description = description;
    }

    public string GetDescription() => $"{Name} ({Description})";
}
