using Bogus;
using FCG.Domain.Entities;

namespace FCG.Tests.Domain.Entities;

public class PlayerTests
{
    private readonly Faker _faker;

    public PlayerTests()
    {
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public void Player_DeveCriarComDadosValidos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var displayName = _faker.Name.FirstName();

        // Act
        var player = new Player(userId, displayName);

        // Assert
        Assert.NotEqual(Guid.Empty, player.Id);
        Assert.Equal(userId, player.UserId);
        Assert.Equal(displayName, player.DisplayName);
        Assert.NotNull(player.Library);
        Assert.Empty(player.Library);
    }

    [Fact]
    public void AlterarDisplayName_DeveAtualizarNome()
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), _faker.Name.FirstName());
        var novoDisplayName = _faker.Name.FirstName();

        // Act
        player.DisplayNameChange(novoDisplayName);

        // Assert
        Assert.Equal(novoDisplayName, player.DisplayName);
    }

    [Fact]
    public void AdicionarJogo_DeveAdicionarJogoNaBiblioteca()
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), _faker.Name.FirstName());
        var game = new Game(
            string.Concat(_faker.Random.Digits(13)),
            _faker.Commerce.ProductName(),
            _faker.Random.Word(),
            _faker.Lorem.Sentence()
        );

        // Act
        // player.AddGame(game);

        // Assert
        Assert.Single(player.Library);
        Assert.Contains(game, player.Library);
    }

    [Fact]
    public void AdicionarJogo_NaoDeveAdicionarJogoDuplicado()
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), _faker.Name.FirstName());
        var game = new Game(
            string.Concat(_faker.Random.Digits(13)), // EAN com 13 dígitos
            _faker.Commerce.ProductName(),
            _faker.Random.Word(),
            _faker.Lorem.Sentence()
        );

        // Act
        // player.AddGame(game);
        // player.AddGame(game);

        // Assert
        Assert.Single(player.Library);
    }

    [Fact]
    public void AdicionarJogo_DevePermitirMultiplosJogosDiferentes()
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), _faker.Name.FirstName());
        var gameFaker = new Faker<Game>()
            .CustomInstantiator(f => new Game(
                string.Concat(f.Random.Digits(13)), // EAN com 13 dígitos
                f.Commerce.ProductName(),
                f.PickRandom("RPG", "FPS", "Adventure", "Strategy"),
                f.Lorem.Sentence()
            ));

        var games = gameFaker.Generate(5);

        // Act
        foreach (var game in games)
        {
            // player.AddGame(game);
        }

        // Assert
        Assert.Equal(5, player.Library.Count);
    }
}