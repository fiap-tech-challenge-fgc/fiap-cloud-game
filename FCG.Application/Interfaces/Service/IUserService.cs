using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Domain.Enums;

namespace FCG.Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserInfoResponseDto>> GetAllAsync(PagedRequestDto<UserFilterDto, UserOrderDto> pagedRequestDto);
        Task<PagedResult<UserInfoResponseDto>> GetUsersByRoleAsync(Roles role, PagedRequestDto<UserFilterDto, UserOrderDto> pagedRequestDto);
        Task<UserInfoResponseDto?> GetByIdAsync(Guid userId);
        Task<OperationResult> UpdateUserAsync(Guid userId, UserUpdateRequestDto dto);
        Task<OperationResult> UpdatePasswordAsync(Guid userId, UserUpdateRequestDto dto);
        Task<OperationResult> DeleteUserAsync(Guid id);
    }
}