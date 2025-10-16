namespace FCG.Infrastructure.Interfaces;

public interface IInfrastructureInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
