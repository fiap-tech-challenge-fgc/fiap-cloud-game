using FCG.Application.Interfaces.Service;
using FCG.Domain.Data.Contexts;
using FCG.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            var identityDb = sp.GetRequiredService<UserDbContext>();
            if (identityDb != null)
            {
                await ApplyDatabaseSchemaAsync(identityDb, "UserDbContext", cancellationToken);
            }

            var appDb = sp.GetService<FcgDbContext>();
            if (appDb != null)
            {
                await ApplyDatabaseSchemaAsync(appDb, "FcgDbContext", cancellationToken);
            }

            var seedService = sp.GetRequiredService<ISeedService>();
            await seedService.SeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar infraestrutura (migrations/seeds)");
            throw;
        }
    }

    private async Task ApplyDatabaseSchemaAsync(DbContext context, string contextName, CancellationToken cancellationToken)
    {
        try
        {
            // Tenta aplicar migrations (funciona apenas para bancos relacionais)
            await context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Migrations aplicadas para {ContextName}", contextName);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Relational-specific"))
        {
            // É um banco InMemory, apenas garante que está criado
            await context.Database.EnsureCreatedAsync(cancellationToken);
            _logger.LogInformation("Banco InMemory criado para {ContextName}", contextName);
        }
    }
}
