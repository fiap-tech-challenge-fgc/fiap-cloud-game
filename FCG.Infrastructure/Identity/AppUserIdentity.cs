using FCG.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FCG.Infrastructure.Identity;

public class AppUserIdentity : IdentityUser<Guid>, IUser
{
    Guid IUser.Id => Id;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get => FirstName + " " + LastName; }
    public string? DisplayName { get; set; }
    public DateOnly Birthday { get; set; }
    string IUser.DisplayName => DisplayName ?? UserName!;
    DateOnly IUser.Birthday => this.Birthday;
    public AppUserIdentity() : base() { }
}
