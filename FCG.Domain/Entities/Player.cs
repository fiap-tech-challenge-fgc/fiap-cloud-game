namespace FCG.Domain.Entities;
public class Player
{
    public Guid Id { get; private set; }
    public string DisplayName { get; private set; }
    public List<Game> Biblioteca { get; private set; }

    private Player() {
        DisplayName = string.Empty;
        Biblioteca = new List<Game>();
    }

    public Player(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
        Biblioteca = new List<Game>();
    }

    public void AlterarDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    public void AdicionarJogo(Game jogo)
    {
        if (!Biblioteca.Contains(jogo))
            Biblioteca.Add(jogo);
    }
}