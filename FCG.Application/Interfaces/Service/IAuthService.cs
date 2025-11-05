using FCG.Application.Dto.Response;
using FCG.Domain.Entities;
using System.Security.Claims;
using FCG.Application.Dto.Result;
using FCG.Application.Dto.Request;

namespace FCG.Application.Interfaces.Service;

public interface IAuthService
{
    Task<OperationResult<User>> RegisterUserAsync(UserCreateRequestDto dto, string role);
    Task<OperationResult<UserAuthResponseDto>> LoginAsync(UserLoginRequestDto dto);
    Task<OperationResult<UserAuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
    Task<OperationResult<UserInfoResponseDto>> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
    Task<OperationResult> UpdatePasswordAsync(Guid userId, UserUpdateRequestDto dto);
}