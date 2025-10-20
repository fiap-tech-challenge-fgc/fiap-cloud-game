namespace FCG.Application.Interfaces.Service;
public interface ISeedService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}