using FCG.Application.Interfaces.Service;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FCG.Application.Services;

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
            var userManager = sp.GetRequiredService<UserManager<User>>();

            var roles = Enum.GetValues(typeof(Roles)).Cast<Roles>();

            foreach (var role in roles)
            {
                var roleName = role.ToString();

                if (cancellationToken.IsCancellationRequested) return;

                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    if (!r.Succeeded)
                    {
                        _logger.LogWarning("Falha ao criar role {Role}: {Errors}", roleName, string.Join(", ", r.Errors.Select(e => e.Description)));
                    }
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


    private List<User> GetPreconfiguredUsers()
    {
        return new List<User>
        {
            new User
            {
                FirstName = "Admin",
                LastName = "Fiap Cloud Games",
                DisplayName = "Admin FCG",
                UserName = "Admin",
                Email = "admin@fiap.com.br"
            },
            new User
            {
                FirstName = "Marcelo",
                LastName = "Mendes Oliveira",
                DisplayName = "Marcelo M.",
                UserName = "rm367563",
                Email = "rm367563@fiap.com.br"
            },
            new User
            {
                FirstName = "Miguel",
                LastName = "de Oliveira Gonçalves",
                DisplayName = "Miguel O.",
                UserName = "rm367985",
                Email = "rm367985@fiap.com.br"
            },
            new User
            {
                FirstName = "Jhonatan",
                LastName = "Brayan",
                DisplayName = "Jhonatan B.",
                UserName = "rm366874",
                Email = "rm366874@fiap.com.br"
            },
            new User
            {
                FirstName = "Matias",
                LastName = "José dos Santos Neto",
                DisplayName = "Matias J.",
                UserName = "rm368795",
                Email = "rm368795@fiap.com.br"
            },
            new User
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
