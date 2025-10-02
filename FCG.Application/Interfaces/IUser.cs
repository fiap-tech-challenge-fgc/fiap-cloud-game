namespace FCG.Application.Interfaces;

public interface IUser
{
    Guid Id { get; }
    DateOnly Birthday { get; }
    string DisplayName { get; }
}
