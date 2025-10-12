using System.ComponentModel.DataAnnotations;

namespace FCG.Api.Interfaces
{
    public interface IAuthenticableUser : IUser
    {
        string PasswordHash { get; set; }
    }
}
