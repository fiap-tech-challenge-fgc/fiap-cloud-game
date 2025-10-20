using FCG.Application.Dtos;
using FCG.Application.Dtos.Response;
using FCG.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FCG.Application.Interfaces.Service;

public interface IAuthService
{
    Task<(IdentityResult Result, User User)> RegisterUserAsync(UserCreateDto dto, string role);
    Task<UserAuthResponseDto> LoginAsync(UserLoginDto dto);
    Task<UserAuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    Task<UserInfoDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
    Task<OperationResult> UpdatePasswordAsync(Guid userId, UserUpdateDto dto);
}

