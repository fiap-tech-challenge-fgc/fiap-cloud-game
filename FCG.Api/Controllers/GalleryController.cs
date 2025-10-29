using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GalleryGameResponseDto>>> GetGalleryGames([FromQuery] PagedRequestDto<GameFilterDto, GameOrderDto> request)
    {
        var result = await _galleryService.GetGalleryGamesAsync(request);

        if (!result.Succeeded)
            return BadRequest(new { result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GalleryGameResponseDto>> GetGalleryGame(Guid id)
    {
        var result = await _galleryService.GetGalleryGameByIdAsync(id);

        if (!result.Succeeded)
            return NotFound(new { result.Errors });
        
        return Ok(result.Data);
    }

    [HttpGet("promotions")]
    public async Task<ActionResult<IEnumerable<GalleryGameResponseDto>>> GetPromotionalGames()
    {
        var result = await _galleryService.GetPromotionalGamesAsync();

        if (!result.Succeeded)
            return NotFound(new { result.Errors });

        return Ok(result.Data);
    }
}