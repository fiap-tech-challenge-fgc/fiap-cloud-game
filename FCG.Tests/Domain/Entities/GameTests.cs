namespace FCG.Tests.Domain.Entities;

public class GameTests
{
    [Fact]
    public void Construtor_DadosValidos_DeveCriarGame()
    {
        // Arrange & Act
        var game = new Game("The Witcher 3", "RPG", "Epic adventure", 59.99m);

        // Assert
        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("The Witcher 3", game.Name);
        Assert.Equal("RPG", game.Genre);
        Assert.Equal("Epic adventure", game.Description);
        Assert.Equal(59.99m, game.Price);
    }

    [Theory]
    [InlineData("", "RPG", "Description", 50)]
    [InlineData(null, "RPG", "Description", 50)]
    [InlineData("   ", "RPG", "Description", 50)]
    public void Construtor_NomeInvalido_DeveLancarExcecao(string nome, string genre, string description, decimal price)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Game(nome, genre, description, price));
    }

    [Theory]
    [InlineData("Game", "", "Description", 50)]
    [InlineData("Game", null, "Description", 50)]
    [InlineData("Game", "   ", "Description", 50)]
    public void Construtor_GeneroInvalido_DeveLancarExcecao(string nome, string genre, string description, decimal price)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Game(nome, genre, description, price));
    }

    [Fact]
    public void Construtor_PrecoNegativo_DeveLancarExcecao()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Game("Game", "Action", "Description", -10));
    }

    [Fact]
    public void PrecoFinal_SemPromocao_DeveRetornarPrecoOriginal()
    {
        // Arrange
        var game = new Game("Game", "Action", null, 100m);

        // Act & Assert
        Assert.Equal(100m, game.PrecoFinal);
    }

    [Fact]
    public void PrecoFinal_ComPromocaoAtiva_DeveAplicarDesconto()
    {
        // Arrange
        var game = new Game("Game", "Action", null, 100m);
        var promocao = Promocao.Criar(TipoPromocao.DescontoFixo, 20m, 
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        // Act
        game.AplicarPromocao(promocao);

        // Assert
        Assert.Equal(80m, game.PrecoFinal);
    }

    [Fact]
    public void AplicarPromocao_PromocaoValida_DeveDefinirPromocao()
    {
        // Arrange
        var game = new Game("Game", "Action", null, 100m);
        var promocao = Promocao.Criar(TipoPromocao.DescontoPercentual, 10m,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        // Act
        game.AplicarPromocao(promocao);

        // Assert
        Assert.Equal(90m, game.PrecoFinal);
    }

    [Fact]
    public void AplicarPromocao_PromocaoNula_DeveDefinirPromocaoNenhuma()
    {
        // Arrange
        var game = new Game("Game", "Action", null, 100m);

        // Act
        game.AplicarPromocao(null);

        // Assert
        Assert.Equal(100m, game.PrecoFinal);
    }

    [Fact]
    public void GetDescription_DeveRetornarDescricaoFormatada()
    {
        // Arrange
        var game = new Game("Cyberpunk 2077", "RPG", "Futuristic RPG", 59.99m);

        // Act
        var description = game.GetDescription();

        // Assert
        Assert.Contains("Cyberpunk 2077", description);
        Assert.Contains("RPG", description);
    }
}