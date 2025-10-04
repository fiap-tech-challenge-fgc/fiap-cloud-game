using FCG.Infrastructure.Data;
using FCG.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Infrastructure.Extensions;

public static class ConfigureIdentityExtensions
{
    public static void AddIdentity(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DbFcg") ;

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
    }
}

