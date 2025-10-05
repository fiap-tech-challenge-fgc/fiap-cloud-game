using FCG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FCG.Infrastructure.Extensions.Builder;
public static class ConfigureDatabaseExtensions
{
    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {        
        var connectionString = builder.Configuration.GetConnectionString("DbFcg");

        builder.Services.AddDbContext<FcgDbContext>(options =>
            options.UseNpgsql(connectionString));

        return builder;
    }
}
