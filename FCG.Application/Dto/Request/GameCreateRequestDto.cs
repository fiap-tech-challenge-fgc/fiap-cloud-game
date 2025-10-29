using System.ComponentModel.DataAnnotations;

namespace FCG.Application.Dto.Request;

public class GameCreateRequestDto
{
    [Required(ErrorMessage = "EAN do jogo é obrigatório")]
    [StringLength(13, MinimumLength = 13, ErrorMessage = "O nome deve ter 13 caracteres")]
    [RegularExpression(@"^\d{13}$", ErrorMessage = "EAN deve conter exatamente 13 dígitos numéricos.")]
    public string EAN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome do jogo é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gênero é obrigatório")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "O gênero deve ter entre 2 e 50 caracteres")]
    public string Genre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.00, 9999.99, ErrorMessage = "O preço deve estar entre R$ 0,00(grátis) e R$ 9.999,99")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
}