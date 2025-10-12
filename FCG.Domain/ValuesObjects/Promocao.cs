using FCG.Domain.ValuesObjects;

public enum TipoPromocao
{
    Nenhuma,
    DescontoFixo,
    DescontoPercentual,
    Bundle
}

public sealed class Promocao : ValueObject
{
    public TipoPromocao Tipo { get; }
    public decimal Valor { get; }
    public DateTime Inicio { get; }
    public DateTime Fim { get; }

    private Promocao(TipoPromocao tipo, decimal valor, DateTime inicio, DateTime fim)
    {
        Tipo = tipo;
        Valor = valor;
        Inicio = inicio;
        Fim = fim;
    }

    public static Promocao Nenhuma => new Promocao(TipoPromocao.Nenhuma, 0, DateTime.MinValue, DateTime.MinValue);

    public static Promocao Criar(TipoPromocao tipo, decimal valor, DateTime inicio, DateTime fim)
    {
        if (tipo != TipoPromocao.Nenhuma && fim <= inicio)
            throw new ArgumentException("Data de fim deve ser posterior à data de início.");

        return new Promocao(tipo, valor, inicio, fim);
    }

    public bool EstaAtiva(DateTime agora) => Tipo != TipoPromocao.Nenhuma && agora >= Inicio && agora <= Fim;

    public decimal AplicarDesconto(decimal precoOriginal)
    {
        if (!EstaAtiva(DateTime.UtcNow)) return precoOriginal;

        return Tipo switch
        {
            TipoPromocao.DescontoFixo => precoOriginal - Valor,
            TipoPromocao.DescontoPercentual => precoOriginal * (1 - Valor / 100),
            _ => precoOriginal
        };
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Tipo;
        yield return Valor;
        yield return Inicio;
        yield return Fim;
    }
}