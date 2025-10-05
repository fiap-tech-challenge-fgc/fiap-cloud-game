namespace FCG.Infrastructure.Services.Seed;
public interface ISeedService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}