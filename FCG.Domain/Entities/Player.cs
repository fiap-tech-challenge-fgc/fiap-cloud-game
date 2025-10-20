using FCG.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; }
    public string DisplayName { get; private set; }
    public List<Game> Library { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private Player()
    {
        DisplayName = string.Empty;
        Library = new List<Game>();
    }

    public Player(Guid userId, string displayName)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        DisplayName = displayName;
        Library = new List<Game>();
    }

    public void AlterarDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    public void AdicionarJogo(Game jogo)
    {
        if (!Library.Contains(jogo))
            Library.Add(jogo);
    }
}