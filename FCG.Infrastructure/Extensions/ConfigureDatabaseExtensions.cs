using FCG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FCG.Infrastructure.Extensions;
public static class ConfigureDatabaseExtensions
{
    public static void AddDatabase(this IHostApplicationBuilder builder)
    {        
        var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

        builder.Services.AddDbContext<FcgDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}
