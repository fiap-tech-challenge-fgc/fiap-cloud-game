namespace FCG.Tests.Domain.ValuesObjects;

public class PromocaoTests
{
    [Fact]
    public void Criar_PromocaoValida_DeveRetornarPromocao()
    {
        // Arrange
        var inicio = DateTime.UtcNow;
        var fim = inicio.AddDays(7);

        // Act
        var promocao = Promocao.Criar(TipoPromocao.DescontoFixo, 10m, inicio, fim);

        // Assert
        Assert.Equal(TipoPromocao.DescontoFixo, promocao.Tipo);
        Assert.Equal(10m, promocao.Valor);
        Assert.Equal(inicio, promocao.Inicio);
        Assert.Equal(fim, promocao.Fim);
    }

    [Fact]
    public void Criar_DataFimAnteriorAInicio_DeveLancarExcecao()
    {
        // Arrange
        var inicio = DateTime.UtcNow;
        var fim = inicio.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Promocao.Criar(TipoPromocao.DescontoFixo, 10m, inicio, fim));
    }

    [Fact]
    public void EstaAtiva_PromocaoAtiva_DeveRetornarTrue()
    {
        // Arrange
        var inicio = DateTime.UtcNow.AddDays(-1);
        var fim = DateTime.UtcNow.AddDays(1);
        var promocao = Promocao.Criar(TipoPromocao.DescontoPercentual, 20m, inicio, fim);

        // Act
        var resultado = promocao.EstaAtiva(DateTime.UtcNow);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void EstaAtiva_PromocaoExpirada_DeveRetornarFalse()
    {
        // Arrange
        var inicio = DateTime.UtcNow.AddDays(-10);
        var fim = DateTime.UtcNow.AddDays(-1);
        var promocao = Promocao.Criar(TipoPromocao.DescontoPercentual, 20m, inicio, fim);

        // Act
        var resultado = promocao.EstaAtiva(DateTime.UtcNow);

        // Assert
        Assert.False(resultado);
    }

    [Theory]
    [InlineData(100, 10, TipoPromocao.DescontoFixo, 90)]
    [InlineData(100, 20, TipoPromocao.DescontoPercentual, 80)]
    [InlineData(50, 15, TipoPromocao.DescontoFixo, 35)]
    public void AplicarDesconto_PromocaoAtiva_DeveCalcularCorretamente(
        decimal precoOriginal, decimal valorDesconto, TipoPromocao tipo, decimal precoEsperado)
    {
        // Arrange
        var inicio = DateTime.UtcNow.AddDays(-1);
        var fim = DateTime.UtcNow.AddDays(1);
        var promocao = Promocao.Criar(tipo, valorDesconto, inicio, fim);

        // Act
        var resultado = promocao.AplicarDesconto(precoOriginal);

        // Assert
        Assert.Equal(precoEsperado, resultado);
    }

    [Fact]
    public void AplicarDesconto_PromocaoInativa_DeveRetornarPrecoOriginal()
    {
        // Arrange
        var inicio = DateTime.UtcNow.AddDays(-10);
        var fim = DateTime.UtcNow.AddDays(-1);
        var promocao = Promocao.Criar(TipoPromocao.DescontoFixo, 10m, inicio, fim);
        var precoOriginal = 100m;

        // Act
        var resultado = promocao.AplicarDesconto(precoOriginal);

        // Assert
        Assert.Equal(precoOriginal, resultado);
    }

    [Fact]
    public void Nenhuma_DeveRetornarPromocaoInativa()
    {
        // Act
        var promocao = Promocao.Nenhuma;

        // Assert
        Assert.Equal(TipoPromocao.Nenhuma, promocao.Tipo);
        Assert.Equal(0m, promocao.Valor);
        Assert.False(promocao.EstaAtiva(DateTime.UtcNow));
    }

    [Fact]
    public void Equals_PromocoesIguais_DeveRetornarTrue()
    {
        // Arrange
        var inicio = new DateTime(2025, 1, 1);
        var fim = new DateTime(2025, 12, 31);
        var promocao1 = Promocao.Criar(TipoPromocao.DescontoFixo, 10m, inicio, fim);
        var promocao2 = Promocao.Criar(TipoPromocao.DescontoFixo, 10m, inicio, fim);

        // Act & Assert
        Assert.Equal(promocao1, promocao2);
    }
}