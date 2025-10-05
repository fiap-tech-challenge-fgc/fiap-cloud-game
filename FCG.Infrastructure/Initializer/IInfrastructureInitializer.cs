namespace FCG.Infrastructure.Initializer;

public interface IInfrastructureInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
