using FCG.Application.Dtos;
using FCG.Domain.Enums;

namespace FCG.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserInfoDto>> GetAllAsync();
    Task<UserInfoDto?> GetByIdAsync(Guid id);
    Task<OperationResult> UpdateUserAsync(Guid id, UserUpdateDto dto);
    Task<OperationResult> UpdatePasswordAsync(Guid userId, UserUpdateDto dto);
    Task<IEnumerable<UserInfoDto>> GetUsersByRoleAsync(Roles role);
    Task<OperationResult> DeleteUserAsync(Guid id); 
}