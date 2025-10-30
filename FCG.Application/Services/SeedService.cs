using FCG.Application.Interfaces.Service;
using FCG.Domain.Data.Contexts;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                    // user.FirstName = adminData.FirstName;
                    // user.LastName = adminData.LastName;
                    // user.UserName = adminData.UserName;
                    // user.DisplayName = adminData.DisplayName;
                    // user.Email = adminData.Email;
                    // user.EmailConfirmed = true;
                    // user.IsActive = true;
                    // 
                    // var result = await userManager.UpdateAsync(user);
                    // 
                    // if (!result.Succeeded)
                    // {
                    //     _logger.LogWarning("Falha ao atualizar o usuário {Email}: {Errors}", adminData, string.Join(", ", result.Errors.Select(e => e.Description)));
                    // }
                }
            }

            _logger.LogInformation("Seed de identidade concluída");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante execução do seed de identidade");
            throw;
        }

        try
        {
            // Seed de games
            _logger.LogInformation("Iniciando seed de games");

            var fcgDbContext = sp.GetRequiredService<FcgDbContext>();

            var games = GetPreconfiguredGames().ToList();

            foreach (var game in games)
            {
                var exists = await fcgDbContext.Games.AnyAsync(g => g.Name == game.Name);
                if (!exists)
                {
                    await fcgDbContext.Games.AddAsync(game, cancellationToken);
                    _logger.LogInformation($"Game Adicionado na base. Nome: {game.Name}");
                }
                
                await fcgDbContext.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Seed de games concluída. {Count} jogos adicionados", games.Count);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante execução do seed de games");
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

    private List<Game> GetPreconfiguredGames()
    {
        return new List<Game>
        {
            new Game
            (
                name: "The Legend of Zelda: Breath of the Wild",
                genre: "Action-Adventure",
                description: "Um épico jogo de aventura em mundo aberto onde você explora o reino de Hyrule e enfrenta o mal.",
                price: 299.90m
            ),
            new Game
            (
                name: "FIFA26",
                genre: "ESports",
                description: "A Copa do Mundo da FIFA 26™ será a 23a edição do torneio de Futebol, mas a primeira com 48 equipes e 3 países sedes : Canadá, Estados Unidos e México",
                price: 279.90m
            ),
            new Game
            (
                name: "God of War Ragnarök",
                genre: "Action",
                description: "Continue a jornada de Kratos e Atreus pelos reinos nórdicos em busca de respostas e batalhas épicas.",
                price: 349.90m
            ),
            new Game
            (
                name: "Elden Ring",
                genre: "RPG",
                description: "Um RPG de ação desafiador criado por FromSoftware e George R.R. Martin em um vasto mundo de fantasia sombria.",
                price: 279.90m
            ),
            new Game
            (
                name: "Minecraft",
                genre: "Sandbox",
                description: "O famoso jogo de construção e sobrevivência onde você pode criar qualquer coisa que imaginar.",
                price: 89.90m
            ),
            new Game
            (
                name: "AstroBot",
                genre: "Action",
                description: "No jogo, o jogador controla Astro em uma missão para resgatar os Lost Bots, recuperar partes da nave-mãe do PlayStation 5 e derrotar o alienígena Space Bully Nebulax, responsável por destruir a nave.",
                price: 339.90m
            ),

        };
    }
}
