using Bogus;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using FCG.Domain.ValuesObjects;

namespace FCG.Tests.Domain.Entities;

public class GameTests
{
    private readonly Faker _faker;

    public GameTests()
    {
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public void Game_DeveCriarComDadosValidos()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var genre = _faker.PickRandom("RPG", "FPS", "Adventure");
        var description = _faker.Lorem.Sentence();
        var price = _faker.Random.Decimal(10, 200);

        // Act
        var game = new Game(name, genre, description, price);

        // Assert
        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal(name, game.Name);
        Assert.Equal(genre, game.Genre);
        Assert.Equal(description, game.Description);
        Assert.Equal(price, game.Price);
        Assert.NotNull(game.Promotion);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Game_DeveLancarExcecao_QuandoNomeInvalido(string nomeInvalido)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Game(nomeInvalido, "RPG", "Description", 50m)
        );

        Assert.Equal("Nome obrigatório", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Game_DeveLancarExcecao_QuandoGeneroInvalido(string generoInvalido)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Game("Game Name", generoInvalido, "Description", 50m)
        );

        Assert.Equal("Gênero obrigatório", exception.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Game_DeveLancarExcecao_QuandoPrecoNegativo(decimal precoInvalido)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Game("Game Name", "RPG", "Description", precoInvalido)
        );

        Assert.Equal("Preço inválido", exception.Message);
    }

    [Fact]
    public void PrecoFinal_DeveRetornarPrecoOriginal_QuandoSemPromocao()
    {
        // Arrange
        var price = 100m;
        var game = new Game(_faker.Commerce.ProductName(), "RPG", null, price);

        // Act
        var precoFinal = game.PrecoFinal;

        // Assert
        Assert.Equal(price, precoFinal);
    }

    [Fact]
    public void AplicarPromocao_DeveAplicarDescontoPercentual()
    {
        // Arrange
        var price = 100m;
        var game = new Game(_faker.Commerce.ProductName(), "RPG", null, price);
        var promocao = Promotion.Criar(
            PromotionType.PercentageDiscount,
            20m,
            DateTime.Now.AddDays(-1),
            DateTime.Now.AddDays(1)
        );

        // Act
        game.AplicarPromocao(promocao);

        // Assert
        Assert.Equal(80m, game.PrecoFinal);
    }

    [Fact]
    public void AplicarPromocao_DeveAplicarDescontoFixo()
    {
        // Arrange
        var price = 100m;
        var game = new Game(_faker.Commerce.ProductName(), "RPG", null, price);
        var promocao = Promotion.Criar(
            PromotionType.FixedDiscount,
            30m,
            DateTime.Now.AddDays(-1),
            DateTime.Now.AddDays(1)
        );

        // Act
        game.AplicarPromocao(promocao);

        // Assert
        Assert.Equal(70m, game.PrecoFinal);
    }

    [Fact]
    public void GetDescription_DeveRetornarDescricaoFormatada()
    {
        // Arrange
        var name = "Cyberpunk 2077";
        var genre = "RPG";
        var price = 199.90m;
        var game = new Game(name, genre, "Descrição", price);

        // Act
        var description = game.GetDescription();

        // Assert
        Assert.Contains(name, description);
        Assert.Contains(genre, description);
        Assert.Contains(price.ToString("C"), description);
    }
}