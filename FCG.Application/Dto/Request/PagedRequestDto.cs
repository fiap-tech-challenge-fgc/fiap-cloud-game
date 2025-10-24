using System.ComponentModel.DataAnnotations;

namespace FCG.Application.Dto.Request
{
    public class PagedRequestDto<TFilter, TOrder>
    {
        [Range(1, int.MaxValue, ErrorMessage = "O número da página deve ser maior ou igual a 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100.")]
        public int PageSize { get; set; } = 10;

        public TOrder? OrderBy { get; set; } // Objeto genérico para ordenação
        public TFilter? Filter { get; set; } // Objeto genérico para filtros
    }
}