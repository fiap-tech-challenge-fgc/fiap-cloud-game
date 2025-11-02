using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Interfaces;
using FCG.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers.Admin;

[Route("api/admin/user")]
[ApiController]
[Authorize(Roles = RoleConstants.Admin)]
public class AdminUserController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUserController(IUserService userService) {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }

    // 👤 Lista todos usuários
    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] PagedRequestDto<UserFilterDto, UserOrderDto> dto)
    {
        var users = await _userService.GetAllAsync(dto);

        return Ok(users.Data);
    }
}
