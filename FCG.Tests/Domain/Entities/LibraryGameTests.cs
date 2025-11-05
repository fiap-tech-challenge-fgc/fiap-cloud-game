using FCG.Domain.Entities;
using System.Numerics;

namespace FCG.Tests.Domain.Entities;

/// <summary>
/// Testes unitários para a entidade LibraryGame
/// </summary>
public class LibraryGameTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_DeveCriarLibraryGameValido_QuandoParametrosValidos()
    {
        // Arrange
        var game = new Game("7891234560001", "Test Game", "Action", "Description");
        var gallery = new GalleryGame(game, 59.99m);
        var player = new Player(Guid.NewGuid(), "TestPlayer");
        decimal purchasePrice = 49.99m;

        // Act
        var libraryGame = new LibraryGame(game.Id, player.Id, purchasePrice);

        // Assert
        Assert.NotEqual(Guid.Empty, libraryGame.Id);
        Assert.Equal(gallery.Id, libraryGame.GalleryId);
        Assert.Equal(gallery, libraryGame.Gallery);
        Assert.Equal(player.Id, libraryGame.PlayerId);
        Assert.Equal(player, libraryGame.Player);
        Assert.Equal(purchasePrice, libraryGame.PurchasePrice);
        Assert.True(libraryGame.PurchaseDate <= DateTime.UtcNow);
        Assert.True(libraryGame.PurchaseDate >= DateTime.UtcNow.AddSeconds(-5)); // Margem de 5 segundos
    }

    [Fact]
    public void Constructor_DeveLancarExcecao_QuandoPrecoNegativo()
    {
        // Arrange
        var game = new Game("7891234560001", "Test Game", "Action", "Description");
        var player = new Player(Guid.NewGuid(), "TestPlayer");
        decimal precoInvalido = -10m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new LibraryGame(game.Id, player.Id, precoInvalido));
        Assert.Contains("Preço de compra inválido", exception.Message);
    }

    [Fact]
    public void Constructor_DeveAceitarPrecoZero()
    {
        // Arrange
        var game = new Game("7891234560001", "Free Game", "Action", "Description");
        var player = new Player(Guid.NewGuid(), "TestPlayer");
        decimal precoZero = 0m;

        // Act
        var libraryGame = new LibraryGame(game.Id, player.Id, precoZero);

        // Assert
        Assert.Equal(0m, libraryGame.PurchasePrice);
    }

    [Fact]
    public void Constructor_DeveDefinirDataDeCompra_ComoDataAtual()
    {
        // Arrange
        var game = new Game("7891234560001", "Test Game", "Action", "Description");
        var player = new Player(Guid.NewGuid(), "TestPlayer");
        var beforeCreation = DateTime.UtcNow;

        // Act
        var libraryGame = new LibraryGame(game.Id, player.Id, 59.99m);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(libraryGame.PurchaseDate, beforeCreation, afterCreation);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Properties_DeveArmazenarValoresCorretamente()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var game = new Game("7891234560001", "Epic Game", "RPG", "Epic description");
        var player = new Player(playerId, "EpicPlayer");
        decimal price = 99.99m;

        // Act
        var libraryGame = new LibraryGame(game.Id, player.Id, price);

        // Assert
        Assert.Equal(game.Id, libraryGame.GalleryId);
        Assert.Equal(player.Id, libraryGame.PlayerId);
        Assert.Equal(price, libraryGame.PurchasePrice);
        Assert.NotNull(libraryGame.Gallery);
        Assert.NotNull(libraryGame.Player);
    }

    [Fact]
    public void PurchaseDate_DeveSerImmutable()
    {
        // Arrange
        var game = new Game("7891234560001", "Test Game", "Action", "Description");
        var player = new Player(Guid.NewGuid(), "TestPlayer");
        var libraryGame = new LibraryGame(game.Id, player.Id, 49.99m);
        var originalDate = libraryGame.PurchaseDate;

        // Act
        System.Threading.Thread.Sleep(100); // Pequeno delay

        // Assert
        Assert.Equal(originalDate, libraryGame.PurchaseDate); // Não deve mudar
    }

    #endregion

    #region GetDescription Tests

    [Fact]
    public void GetDescription_DeveRetornarDescricaoCompleta()
    {
        // Arrange
        var game = new Game("7891234560001", "Amazing Game", "Action", "Super fun game");
        var player = new Player(Guid.NewGuid(), "GamerPro");
        var libraryGame = new LibraryGame(game.Id, player.Id, 79.90m);

        // Act
        var description = libraryGame.GetDescription();

        // Assert
        Assert.Contains("Amazing Game", description);
        Assert.Contains("79,90", description); // Preço formatado
        Assert.Contains("Comprado em", description);
    }

    [Fact]
    public void GetDescription_DeveIncluirDataDeCompra()
    {
        // Arrange
        var game = new Game("7891234560001", "Test Game", "Action", "Description");
        var player = new Player(Guid.NewGuid(), "TestPlayer");
        var libraryGame = new LibraryGame(game.Id, player.Id, 49.99m);

        // Act
        var description = libraryGame.GetDescription();

        // Assert
        Assert.Contains(libraryGame.PurchaseDate.ToString("d"), description);
    }

    [Fact]
    public void GetDescription_DeveIncluirPrecoDeCompra()
    {
        // Arrange
        var game = new Game("7891234560001", "Test Game", "Action", "Description");
        var player = new Player(Guid.NewGuid(), "TestPlayer");
        decimal price = 129.99m;
        var libraryGame = new LibraryGame(game.Id, player.Id, price);

        // Act
        var description = libraryGame.GetDescription();

        // Assert
        Assert.Contains("129,99", description); // Formato brasileiro
    }

    [Fact]
    public void GetDescription_DeveFuncionarComJogoComSubtitulo()
    {
        // Arrange
        var game = new Game("7891234560001", "Epic Saga", "RPG", "The best RPG ever", "The Beginning");
        var player = new Player(Guid.NewGuid(), "RPGFan");
        var libraryGame = new LibraryGame(game.Id, player.Id, 199.90m);

        // Act
        var description = libraryGame.GetDescription();

        // Assert
        Assert.Contains("Epic Saga", description);
        Assert.Contains("The Beginning", description);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void MultiplasBibliotecas_DevemTerJogosIndependentes()
    {
        // Arrange
        var game1 = new Game("7891234560001", "Game 1", "Action", "Description 1");
        var game2 = new Game("7891234560002", "Game 2", "RPG", "Description 2");

        var player1 = new Player(Guid.NewGuid(), "Player1");
        var player2 = new Player(Guid.NewGuid(), "Player2");

        // Act
        var library1Game1 = new LibraryGame(game1.Id, player1.Id, 49.99m);
        var library1Game2 = new LibraryGame(game2.Id, player1.Id, 59.99m);
        var library2Game1 = new LibraryGame(game1.Id, player2.Id, 39.99m); // Mesmo jogo, preço diferente

        // Assert
        Assert.Equal(player1.Id, library1Game1.PlayerId);
        Assert.Equal(player1.Id, library1Game2.PlayerId);
        Assert.Equal(player2.Id, library2Game1.PlayerId);

        Assert.Equal(49.99m, library1Game1.PurchasePrice);
        Assert.Equal(39.99m, library2Game1.PurchasePrice); // Preços diferentes

        Assert.Equal(game1.Id, library1Game1.GalleryId);
        Assert.Equal(game1.Id, library2Game1.GalleryId); // Mesmo jogo
    }

    [Fact]
    public void JogoCompradoEmPromocao_DeveRegistrarPrecoPromocional()
    {
        // Arrange
        var game = new Game("7891234560001", "Sale Game", "Action", "Description");
        var player = new Player(Guid.NewGuid(), "BargainHunter");
        decimal originalPrice = 100m;
        decimal promotionalPrice = 49.99m; // 50% off

        // Act
        var libraryGame = new LibraryGame(game.Id, player.Id, promotionalPrice);

        // Assert
        Assert.Equal(promotionalPrice, libraryGame.PurchasePrice);
        Assert.NotEqual(originalPrice, libraryGame.PurchasePrice);
    }

    [Fact]
    public void ComparacaoDePrecos_EntreDiferentesCompras()
    {
        // Arrange
        var game = new Game("7891234560001", "Popular Game", "Action", "Description");
        var player1 = new Player(Guid.NewGuid(), "EarlyBuyer");
        var player2 = new Player(Guid.NewGuid(), "LateBuyer");

        // Act - Player 1 compra no lançamento
        var earlyPurchase = new LibraryGame(game.Id, player1.Id, 199.90m);

        System.Threading.Thread.Sleep(100); // Simula tempo passando

        // Player 2 compra em promoção
        var latePurchase = new LibraryGame(game.Id, player2.Id, 99.90m);

        // Assert
        Assert.True(earlyPurchase.PurchasePrice > latePurchase.PurchasePrice);
        Assert.True(earlyPurchase.PurchaseDate < latePurchase.PurchaseDate);
        Assert.Equal(game.Id, earlyPurchase.GalleryId);
        Assert.Equal(game.Id, latePurchase.GalleryId);
    }

    [Fact]
    public void BibliotecaCompleta_DeveManterHistoricoDeCompras()
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), "Collector");
        var games = new[]
           {
                new Game("7891234560001", "Game 1", "Action", "Desc 1"),
                new Game("7891234560002", "Game 2", "RPG", "Desc 2"),
                new Game("7891234560003", "Game 3", "Strategy", "Desc 3")
            };

        var prices = new[] { 49.99m, 79.99m, 99.99m };

        // Act
        var libraryGames = new List<LibraryGame>();
        for (int i = 0; i < games.Length; i++)
        {
            System.Threading.Thread.Sleep(10); // Pequeno delay entre compras
            libraryGames.Add(new LibraryGame(games[i].Id, player.Id, prices[i]));
        }

        // Assert
        Assert.Equal(3, libraryGames.Count);

        // Verifica ordem cronológica
        Assert.True(libraryGames[0].PurchaseDate <= libraryGames[1].PurchaseDate);
        Assert.True(libraryGames[1].PurchaseDate <= libraryGames[2].PurchaseDate);

        // Verifica total gasto
        var totalSpent = libraryGames.Sum(lg => lg.PurchasePrice);
        Assert.Equal(229.97m, totalSpent);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void LibraryGame_DeveAceitarJogoGratis()
    {
        // Arrange
        var game = new Game("7891234560001", "Free to Play", "Action", "Free game");
        var player = new Player(Guid.NewGuid(), "FreebieCollector");

        // Act
        var libraryGame = new LibraryGame(game.Id, player.Id, 0m);

        // Assert
        Assert.Equal(0m, libraryGame.PurchasePrice);
        var description = libraryGame.GetDescription();
        Assert.Contains("0,00", description);
    }

    [Fact]
    public void LibraryGame_DeveAceitarPrecosAltos()
    {
        // Arrange
        var game = new Game("7891234560001", "Premium Edition", "RPG", "Deluxe version");
        var player = new Player(Guid.NewGuid(), "WhaleGamer");
        decimal premiumPrice = 9999.99m;

        // Act
        var libraryGame = new LibraryGame(game.Id, player.Id, premiumPrice);

        // Assert
        Assert.Equal(premiumPrice, libraryGame.PurchasePrice);
    }

    [Fact]
    public void LibraryGame_DeveManterReferenciaParaGameEPlayer()
    {
        // Arrange
        var game = new Game("7891234560001", "Test Game", "Action", "Description");
        var player = new Player(Guid.NewGuid(), "TestPlayer");

        // Act
        var libraryGame = new LibraryGame(game.Id, player.Id, 49.99m);

        // Assert
        Assert.Same(game, libraryGame.Gallery); // Mesma referência
        Assert.Same(player, libraryGame.Player); // Mesma referência
        Assert.Equal(game.Title, libraryGame.Gallery.Game.Title);
        Assert.Equal(player.DisplayName, libraryGame.Player.DisplayName);
    }

    #endregion
}
