using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FCG.Infrastructure.Interfaces;
using FCG.Domain.Data.Contexts;
using FCG.Application.Interfaces.Service;

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
        try
        {
            _logger.LogInformation("Infraestrutura inicializando seeds.");

            // 1. Identity
            using (var scope = _provider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var identityDb = sp.GetRequiredService<UserDbContext>();

                // Aplica apenas migrations pendentes do Identity
                var pendingMigrations = await identityDb.Database.GetPendingMigrationsAsync(cancellationToken);
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Aplicando migrations do Identity...");
                    await identityDb.Database.MigrateAsync(cancellationToken);
                }

                var seedService = sp.GetRequiredService<ISeedService>();
                await seedService.SeedIdentityAsync(sp, cancellationToken);
            }

            // 2. Aplicação
            using (var scope = _provider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var appDb = sp.GetRequiredService<FcgDbContext>();

                // Apenas garante que o banco existe e aplica migrations pendentes
                var appPendingMigrations = await appDb.Database.GetPendingMigrationsAsync(cancellationToken);
                if (appPendingMigrations.Any())
                {
                    _logger.LogInformation("Aplicando migrations da aplicação...");
                    await appDb.Database.MigrateAsync(cancellationToken);
                }

                var seedService = sp.GetRequiredService<ISeedService>();
                await seedService.SeedApplicationAsync(cancellationToken);
            }

            _logger.LogInformation("Infraestrutura e seeds inicializados com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar infraestrutura (migrations/seeds)");
            throw;
        }
    }
}