using FCG.Infrastructure.Identity;
using System.Security.Claims;

namespace FCG.Infrastructure.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateToken(AppUserIdentity userIdentity);
        Task<string> GenerateRefreshToken(AppUserIdentity userIdentity);
        Task<bool> ValidateRefreshToken(AppUserIdentity userIdentity, string refreshToken);
        ClaimsPrincipal? ValidateToken(string token);
    }
}

