using FCG.Domain.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FCG.Domain.Data.Factories;

public class FcgDbContextFactory : IDesignTimeDbContextFactory<FcgDbContext>
{
    public FcgDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // Caminho até o appsettings.json (ajuste conforme sua estrutura)
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../FCG.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DbFcg")
            ?? "Host=localhost;Port=5432;Database=fcg_local;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<FcgDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new FcgDbContext(optionsBuilder.Options);
    }
}
