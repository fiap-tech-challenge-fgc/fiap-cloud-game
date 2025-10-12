using FCG.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FCG.Infrastructure.Identity;

public class AppUserIdentity : IdentityUser<Guid>, IUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string DisplayName { get; set; } = string.Empty;

    Guid IUser.Id => Id;
    string IUser.FirstName => FirstName;
    string IUser.LastName => LastName;
    DateTime IUser.Birthday => Birthday;
    string IUser.FullName => FullName;
    string IUser.DisplayName => DisplayName;

    public AppUserIdentity() : base() {
        UserName ??= Email;
    }
}