namespace FCG.Application.Interfaces;

public interface IUser
{
    Guid Id { get; }
    string FirstName { get; }
    string LastName { get; }
    string FullName { get; }
    string DisplayName { get; }
    DateTime Birthday { get; }
}
