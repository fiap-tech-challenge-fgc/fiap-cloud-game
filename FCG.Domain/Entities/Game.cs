using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FCG.Domain.Entities;

[Index(nameof(EAN), IsUnique = true)]
public class Game
{
    public Guid Id { get; protected set; }

    [Required]
    public string EAN { get; protected set; }
    public string Name { get; protected set; } = string.Empty;
    public string Genre { get; protected set; } = string.Empty;
    public string? Description { get; protected set; }

    protected Game() { }

    public Game(string ean, string name, string genre, string? description)
    {
        if (string.IsNullOrWhiteSpace(ean)) throw new ArgumentException("EAN obrigatório");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome obrigatório");
        if (string.IsNullOrWhiteSpace(genre)) throw new ArgumentException("Gênero obrigatório");

        Id = Guid.NewGuid();
        EAN =  ean;
        Name = name;
        Genre = genre;
        Description = description;
    }

    public virtual string GetDescription()
    {
        return $"{Name} ({Description})";
    }
}