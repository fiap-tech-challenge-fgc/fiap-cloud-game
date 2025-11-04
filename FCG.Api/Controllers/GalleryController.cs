using FCG.Api.Controllers.Base;
using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GalleryController : ApiControllerBase
{
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGalleryGames([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> request)
    {
        var result = await _galleryService.GetGalleryGamesAsync(request);

        return ProcessResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGalleryGame(Guid id)
    {
        var result = await _galleryService.GetGalleryGameByIdAsync(id);

        if (!result.Succeeded)
            return NotFoundResult<object>(result.Errors);
        
        return ProcessResult(result);
    }

    [HttpGet("promotions")]
    public async Task<IActionResult> GetPromotionalGames()
    {
        var result = await _galleryService.GetPromotionalGamesAsync();

        if (!result.Succeeded)
            return NotFoundResult<object>(result.Errors);

        return ProcessResult(result);
    }
}