using FCG.Application.Dtos;
using FCG.Application.Interfaces;
using FCG.Application.Security;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FCG.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserInfoDto>> GetAllAsync()
    {
        var users = await _userManager.Users
            .Where(u => u.EmailConfirmed)
            .ToListAsync();

        var players = new List<UserInfoDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(RoleConstants.Player))
            {
                players.Add(Map(user));
            }
        }

        return players;
    }

    public async Task<IEnumerable<UserInfoDto>> GetUsersByRoleAsync(Roles role)
    {
        var users = await _userManager.Users
            .Where(u => u.EmailConfirmed)
            .ToListAsync();

        var filteredUsers = new List<UserInfoDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(role.ToString()))
            {
                filteredUsers.Add(Map(user));
            }
        }

        return filteredUsers;
    }

    public async Task<UserInfoDto?> GetByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains(RoleConstants.Player) ? Map(user) : null;
    }

    public async Task<OperationResult> UpdateUserAsync(Guid userId, UserUpdateDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return OperationResult.Failure("Usuário não encontrado.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.DisplayName = dto.DisplayName;
        user.Email = dto.Email;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded ? OperationResult.Success() : OperationResult.Failure("Erro ao atualizar usuário.");
    }

    public async Task<OperationResult> UpdatePasswordAsync(Guid userId, UserUpdateDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return OperationResult.Failure("Usuário não encontrado.");

        // Verifica se a nova senha e a confirmação são iguais
        if (dto.NewPassword != dto.ConfirmNewPassword)
            return OperationResult.Failure("Nova senha e confirmação não conferem.");

        // Tenta alterar a senha usando o método seguro do UserManager
        var result = await _userManager.ChangePasswordAsync(user, dto.Password, dto.NewPassword);

        if (!result.Succeeded)
            return OperationResult.Failure(result.Errors.Select(e => e.Description).ToArray());

        return OperationResult.Success();
    }


    public async Task<OperationResult> DeleteUserAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return OperationResult.Failure("Usuário não encontrado.");

        user.IsActive = false;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? OperationResult.Success() : OperationResult.Failure("Erro ao inativar usuário.");
    }

    private UserInfoDto Map(User user) => new UserInfoDto
    {
        Id = user.Id,
        DisplayName = user.DisplayName,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email!
    };
}