using FCG.Application.Dtos;
using FCG.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly IUserService _userService;

    public PlayerController(
        ILogger<PlayerController> logger,
        IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPut("players/{id}/password")]
    public async Task<IActionResult> UpdatePlayerPassword(Guid id, [FromBody] UserUpdateDto dto)
    {
        var result = await _userService.UpdatePasswordAsync(id, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return NoContent();
    }
}