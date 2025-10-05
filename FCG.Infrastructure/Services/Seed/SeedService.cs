using FCG.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FCG.Infrastructure.Services.Seed;

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

            var adminDatas = new[]
            {
                "admin|admin|admin@fiap.com.br",
                "Marcelo Mendes Oliveira|Marcelo M.|rm367563@fiap.com.br",
                "Miguel de Oliveira Gonçalves|Miguel O.|rm367985@fiap.com.br",
                "Jhonatan Brayan|Jhonatan B.|rm366874@fiap.com.br",
                "Matias José dos Santos Neto|Matias J.|matiasjsneto@gmail.com",
                "João Carlos Silva de Souza|João C.|jocasiso@gmail.com"
            };

            foreach (var adminData in adminDatas)
            {
                if (cancellationToken.IsCancellationRequested) return;

                if (adminData.Split('|').Length != 3)
                {
                    continue;
                }

                string userName = adminData.Split('|')[0];
                string displayName = adminData.Split('|')[1];
                string email = adminData.Split('|')[2];

                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var newUser = new AppUserIdentity
                    {
                        UserName = userName,
                        DisplayName = displayName,
                        Email = email,
                        EmailConfirmed = true
                    };

                    // Atenção: garanta que a senha atende às políticas de IdentityOptions
                    var result = await userManager.CreateAsync(newUser, "SenhaForte@123");

                    if (result.Succeeded)
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(newUser, "Admin");
                        if (!addRoleResult.Succeeded)
                            _logger.LogWarning("Falha ao adicionar usuário {Email} ao role Admin: {Errors}", adminData, string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                    }
                    else
                    {
                        _logger.LogWarning("Falha ao criar usuário {Email}: {Errors}", adminData, string.Join(", ", result.Errors.Select(e => e.Description)));
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
}
