namespace FCG.Infrastructure.Interfaces;
public interface ISeedService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}