namespace FCG.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; }
    public string DisplayName { get; private set; }
    public Guid UserId { get; private set; }
    public IReadOnlyCollection<LibraryGame> Library => _library.AsReadOnly();
    private readonly List<LibraryGame> _library;

    private Player()
    {
        DisplayName = string.Empty;
        _library = new List<LibraryGame>();
    }

    public Player(Guid userId, string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("DisplayName não pode ser vazio.");

        Id = Guid.NewGuid();
        UserId = userId;
        DisplayName = displayName;
        _library = new List<LibraryGame>();
    }

    public void DisplayNameChange(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("DisplayName não pode ser vazio.");
        DisplayName = displayName;
    }

    public LibraryGame AddGame(GalleryGame game)
    {
        if (_library.Any(g => g.Name == game.Name))
            throw new InvalidOperationException("Jogo já existe na biblioteca.");

        var libraryGame = new LibraryGame(game, this, game.FinalPrice);
        _library.Add(libraryGame);
        return libraryGame;
    }
}