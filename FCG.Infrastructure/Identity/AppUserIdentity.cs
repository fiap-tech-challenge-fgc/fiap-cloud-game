using FCG.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FCG.Infrastructure.Identity;

public class AppUserIdentity : IdentityUser<Guid>, IUser
{
    public string? DisplayName { get; set; }
    public DateOnly Birthday { get; set; }

    Guid IUser.Id => Id;
    string IUser.DisplayName => DisplayName ?? UserName!;
    DateOnly IUser.Birthday => this.Birthday;

    public AppUserIdentity() : base() { }
}
