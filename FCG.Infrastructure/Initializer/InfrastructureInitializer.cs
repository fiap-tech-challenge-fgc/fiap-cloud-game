using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FCG.Infrastructure.Data;
using FCG.Infrastructure.Services.Seed;

namespace FCG.Infrastructure.Initializer;

public class InfrastructureInitializer : IInfrastructureInitializer
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<InfrastructureInitializer> _logger;

    public InfrastructureInitializer(IServiceProvider provider, ILogger<InfrastructureInitializer> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        var sp = scope.ServiceProvider;

        try
        {
            var identityDb = sp.GetRequiredService<AppIdentityDbContext>();
            await identityDb.Database.MigrateAsync(cancellationToken);

            var appDb = sp.GetService<FcgDbContext>();
            if (appDb != null)
                await appDb.Database.MigrateAsync(cancellationToken);

            var seedService = sp.GetRequiredService<ISeedService>();
            await seedService.SeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar infraestrutura (migrations/seeds)");
            throw;
        }
    }
}
