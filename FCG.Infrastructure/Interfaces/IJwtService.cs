using FCG.Infrastructure.Identity;
using System.Security.Claims;

namespace FCG.Infrastructure.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateToken(User userIdentity);
        Task<string> GenerateRefreshToken(User userIdentity);
        Task<bool> ValidateRefreshToken(User userIdentity, string refreshToken);
        ClaimsPrincipal? ValidateToken(string token);
    }
}

