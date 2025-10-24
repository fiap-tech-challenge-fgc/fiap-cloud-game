using Bogus;
using FCG.Domain.Entities;

namespace FCG.Tests.Domain.Entities;

public class CartItemTests
{
    private readonly Faker _faker;

    public CartItemTests()
    {
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public void CartItem_DeveCriarComDadosValidos()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        // Act
        var cartItem = new CartItem(playerId, gameId);

        // Assert
        Assert.NotEqual(Guid.Empty, cartItem.Id);
        Assert.Equal(playerId, cartItem.PlayerId);
        Assert.Equal(gameId, cartItem.GameId);
    }

    [Fact]
    public void CartItem_DeveGerarIdUnico()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        // Act
        var cartItem1 = new CartItem(playerId, gameId);
        var cartItem2 = new CartItem(playerId, gameId);

        // Assert
        Assert.NotEqual(cartItem1.Id, cartItem2.Id);
    }

    [Fact]
    public void CartItem_DevePermitirMultiplosItensParaMesmoJogador()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var game1Id = Guid.NewGuid();
        var game2Id = Guid.NewGuid();
        var game3Id = Guid.NewGuid();

        // Act
        var cartItem1 = new CartItem(playerId, game1Id);
        var cartItem2 = new CartItem(playerId, game2Id);
        var cartItem3 = new CartItem(playerId, game3Id);

        // Assert
        Assert.Equal(playerId, cartItem1.PlayerId);
        Assert.Equal(playerId, cartItem2.PlayerId);
        Assert.Equal(playerId, cartItem3.PlayerId);
        Assert.NotEqual(cartItem1.GameId, cartItem2.GameId);
        Assert.NotEqual(cartItem2.GameId, cartItem3.GameId);
    }

    [Fact]
    public void CartItem_DevePermitirMesmoJogoEmCarrinhoDiferentesJogadores()
    {
        // Arrange
        var player1Id = Guid.NewGuid();
        var player2Id = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        // Act
        var cartItem1 = new CartItem(player1Id, gameId);
        var cartItem2 = new CartItem(player2Id, gameId);

        // Assert
        Assert.Equal(gameId, cartItem1.GameId);
        Assert.Equal(gameId, cartItem2.GameId);
        Assert.NotEqual(cartItem1.PlayerId, cartItem2.PlayerId);
        Assert.NotEqual(cartItem1.Id, cartItem2.Id);
    }

    [Fact]
    public void CartItem_DeveCriarComNavigationPropertiesConfiguradas()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        // Act
        var cartItem = new CartItem(playerId, gameId);

        // Assert
        Assert.NotEqual(Guid.Empty, cartItem.Id);
        Assert.NotEqual(Guid.Empty, cartItem.PlayerId);
        Assert.NotEqual(Guid.Empty, cartItem.GameId);
    }

    [Fact]
    public void CartItem_DeveManterConsistenciaEntreIdReferencias()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        // Act
        var cartItem = new CartItem(playerId, gameId);

        // Assert
        Assert.Equal(playerId, cartItem.PlayerId);
        Assert.Equal(gameId, cartItem.GameId);
    }

    [Fact]
    public void CartItem_DevePermitirCriacaoEmLote()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var cartItemFaker = new Faker<CartItem>()
            .CustomInstantiator(f => new CartItem(
                playerId,
                Guid.NewGuid()
            ));

        // Act
        var cartItems = cartItemFaker.Generate(10);

        // Assert
        Assert.Equal(10, cartItems.Count);
        Assert.All(cartItems, item => Assert.Equal(playerId, item.PlayerId));
        Assert.Equal(cartItems.Count, cartItems.Select(c => c.Id).Distinct().Count());
        Assert.Equal(cartItems.Count, cartItems.Select(c => c.GameId).Distinct().Count());
    }
}