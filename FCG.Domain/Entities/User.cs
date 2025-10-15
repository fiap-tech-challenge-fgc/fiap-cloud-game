using Microsoft.AspNetCore.Identity;

namespace FCG.Domain.Entities;
public class User : BaseEntity
{
    public string DisplayName { get; private set; }
    public List<Game> Biblioteca { get; private set; }

    private User() { }

    public User(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
        Biblioteca = new();
    }

    public void AdicionarJogo(Game jogo)
    {
        if (!Biblioteca.Contains(jogo))
            Biblioteca.Add(jogo);
    }
}