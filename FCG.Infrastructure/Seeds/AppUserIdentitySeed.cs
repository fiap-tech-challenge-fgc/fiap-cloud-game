using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FCG.Infrastructure.Identity; // onde está AppUserIdentity

namespace FCG.Infrastructure.Seeds;

public static class AppUserIdentitySeed
{
    public static async Task SeedAppIdentityAsync(this IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUserIdentity>>();

        string[] roles = { "Admin", "Gamer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var adminEmails = new[]
        {
            "admin@fiap.com.br",
            "rm367563@fiap.com.br",
            "rm367985@fiap.com.br",
            "rm366874@fiap.com.br"
        };

        foreach (var email in adminEmails)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var newUser = new AppUserIdentity
                {
                    UserName = email,
                    DisplayName = email.Split("@")[0],
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newUser, "SenhaForte@123");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newUser, "Admin");
            }
        }
    }
}
