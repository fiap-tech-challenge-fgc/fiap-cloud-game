namespace FCG.Application.Dtos.Response;

public class GameListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PrecoFinal { get; set; }
    public bool EmPromocao { get; set; }
    public string TipoPromocao { get; set; } = string.Empty;
    public decimal ValorPromocao { get; set; }
}