// FCG.Infrastructure/Extensions/Builder/ConfigureIdentityExtensions.cs
using FCG.Infrastructure.Data;
using FCG.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FCG.Infrastructure.Extensions.Builder;

public static class IdentityExtensions
{
    public static IHostApplicationBuilder AddIdentity(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DbFcg");

        builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.AddIdentity<AppUserIdentity, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 1;
        })
        .AddEntityFrameworkStores<AppIdentityDbContext>()
        .AddDefaultTokenProviders();

        return builder;
    }
}
