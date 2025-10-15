using FCG.Infrastructure.Identity;
using FCG.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FCG.Infrastructure.Services;

public class SeedService : ISeedService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<SeedService> _logger;

    public SeedService(IServiceProvider provider, ILogger<SeedService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        var sp = scope.ServiceProvider;

        try
        {
            _logger.LogInformation("Iniciando seed de identidade");

            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = sp.GetRequiredService<UserManager<AppUserIdentity>>();

            string[] roles = { "Admin", "Gamer" };

            foreach (var role in roles)
            {
                if (cancellationToken.IsCancellationRequested) return;

                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    if (!r.Succeeded)
                        _logger.LogWarning("Falha ao criar role {Role}: {Errors}", role, string.Join(", ", r.Errors.Select(e => e.Description)));
                }
            }

            var adminDatas = GetPreconfiguredUsers().ToList();

            foreach (var adminData in adminDatas)
            {
                if (cancellationToken.IsCancellationRequested) return;

                var user = await userManager.FindByEmailAsync(adminData.Email!);

                if (user == null)
                {
                    // Atenção: garanta que a senha atende às políticas de IdentityOptions
                    var result = await userManager.CreateAsync(adminData, "SenhaForte@123");

                    if (result.Succeeded)
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(adminData, "Admin");
                        if (!addRoleResult.Succeeded)
                            _logger.LogWarning("Falha ao adicionar usuário {Email} ao role Admin: {Errors}", adminData, string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                    }
                    else
                    {
                        _logger.LogWarning("Falha ao criar usuário {Email}: {Errors}", adminData, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    user.FirstName = adminData.FirstName;
                    user.LastName = adminData.LastName;
                    user.UserName = adminData.UserName;
                    user.DisplayName = adminData.DisplayName;
                    user.Email = adminData.Email;
                    user.EmailConfirmed = true;

                    var result = await userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        _logger.LogWarning("Falha ao atualizar o usuário {Email}: {Errors}", adminData, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            _logger.LogInformation("Seed de identidade concluída");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante execução do seed de identidade");
            throw;
        }
    }


    private List<AppUserIdentity> GetPreconfiguredUsers()
    {
        return new List<AppUserIdentity>
        {
            new AppUserIdentity
            {
                FirstName = "Admin",
                LastName = "Fiap Cloud Games",
                DisplayName = "Admin FCG",
                UserName = "Admin",
                Email = "admin@fiap.com.br"
            },
            new AppUserIdentity
            {
                FirstName = "Marcelo",
                LastName = "Mendes Oliveira",
                DisplayName = "Marcelo M.",
                UserName = "rm367563",
                Email = "rm367563@fiap.com.br"
            },
            new AppUserIdentity
            {
                FirstName = "Miguel",
                LastName = "de Oliveira Gonçalves",
                DisplayName = "Miguel O.",
                UserName = "rm367985",
                Email = "rm367985@fiap.com.br"
            },
            new AppUserIdentity
            {
                FirstName = "Jhonatan",
                LastName = "Brayan",
                DisplayName = "Jhonatan B.",
                UserName = "rm366874",
                Email = "rm366874@fiap.com.br"
            },
            new AppUserIdentity
            {
                FirstName = "Matias",
                LastName = "José dos Santos Neto",
                DisplayName = "Matias J.",
                UserName = "rm368795",
                Email = "rm368795@fiap.com.br"
            },
            new AppUserIdentity
            {
                FirstName = "João",
                LastName = "Carlos Silva de Souza",
                DisplayName = "João C.",
                UserName = "rm367929",
                Email = "rm367929@fiap.com.br"
            }
        };
    }
}
