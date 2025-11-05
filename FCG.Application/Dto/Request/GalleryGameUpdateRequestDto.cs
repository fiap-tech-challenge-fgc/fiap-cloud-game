using System.ComponentModel.DataAnnotations;

namespace FCG.Application.Dto.Request;

public class GalleryGameUpdateRequestDto : GameUpdateRequestDto
{
    public string? PromotionType { get; set; }
    public decimal? PromotionValue { get; set; }
    public DateTime? PromotionStartDate { get; set; }
    public DateTime? PromotionEndDate { get; set; }
}