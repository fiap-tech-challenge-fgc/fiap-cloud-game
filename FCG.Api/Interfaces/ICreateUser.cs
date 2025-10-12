namespace FCG.Api.Interfaces;

public interface ICreateUser : IUser
{
    string Password { get; set; }
    string ConfirmPassword { get; set; }
}
