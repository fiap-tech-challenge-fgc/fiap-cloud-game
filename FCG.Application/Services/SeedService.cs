using FCG.Application.Interfaces.Repository;
using FCG.Application.Interfaces.Service;
using FCG.Domain.Data;
using FCG.Domain.Data.Contexts;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using FCG.Domain.ValuesObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace FCG.Application.Services;

public class SeedService : ISeedService
{
    private readonly ILogger<SeedService> _logger;
    private readonly IGameRepository _gameRepository;
    private readonly IGalleryRepository _galleryRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ILibraryRepository _libraryRepository;

    public SeedService(
        IServiceProvider provider,
        ILogger<SeedService> logger,
        IGameRepository gameRepository,
        IGalleryRepository galleryRepository,
        IPlayerRepository playerRepository,
        ICartRepository cartRepository,
        ILibraryRepository libraryRepository)
    {
        _logger = logger;
        _gameRepository = gameRepository;
        _galleryRepository = galleryRepository;
        _playerRepository = playerRepository;
        _cartRepository = cartRepository;
        _libraryRepository = libraryRepository;
    }

    #region Starter
    public async Task SeedIdentityAsync(IServiceProvider sp, CancellationToken cancellationToken)
    {
        await SeedIdentityRoles(sp, cancellationToken);
        await SeedIdentityAdminAsync(sp, cancellationToken);
        await SeedIdentityPlayerAsync(sp, cancellationToken);
    }

    public async Task SeedApplicationAsync(IServiceProvider sp, CancellationToken cancellationToken)
    {
        // 1. Seed de jogos
        var games = await SeedGamesAsync(cancellationToken);

        // 2. Seed de galerias
        await SeedGalleryGamesAsync(games, cancellationToken);

        // 3. Seed de players (FCG)
        await SeedPlayersAsync(sp, cancellationToken);

        // 4. Seed de carrinhos
        await SeedCartsAsync(cancellationToken);

        // 5. Seed de biblioteca
        await SeedLibraryAsync(cancellationToken);
    }
    #endregion

    #region Identity
    private async Task SeedIdentityRoles(IServiceProvider sp, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Seed de Roles iniciada");

            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var roles = Enum.GetValues(typeof(Roles)).Cast<Roles>();

            foreach (var role in roles)
            {
                if (cancellationToken.IsCancellationRequested) return;
                var roleName = role.ToString();
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    if (!r.Succeeded)
                        _logger.LogWarning("Falha ao criar role {Role}: {Errors}", roleName, string.Join(", ", r.Errors.Select(e => e.Description)));
                }
            }

            _logger.LogInformation("Seed de Roles finalizada");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no Seed de Roles");
        }
    }

    private async Task SeedIdentityAdminAsync(IServiceProvider sp, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Seed de Admin iniciada");

            var userManager = sp.GetRequiredService<UserManager<User>>();
            var adminList = GetPreconfiguredAdminUsers();


            foreach (var admin in adminList)
            {
                if (cancellationToken.IsCancellationRequested) 
                    return;

                var user = await userManager.FindByEmailAsync(admin.Email!);

                if (user == null)
                {
                    var result = await userManager.CreateAsync(admin, "SenhaForte@123");
                    
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
                    else
                        _logger.LogWarning("Falha ao criar usuário {Email}: {Errors}", admin.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            _logger.LogInformation("Seed de Roles finalizada");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no Seed de Admin");
        }
    }

    private async Task SeedIdentityPlayerAsync(IServiceProvider sp, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Seed de User.Player iniciada");

            var userManager = sp.GetRequiredService<UserManager<User>>();
            var playersList = GetPreconfiguredPlayerUsers();

            foreach (var player in playersList)
            {
                if (cancellationToken.IsCancellationRequested) 
                    return;

                var user = await userManager.FindByEmailAsync(player.Email!);

                if (user == null)
                {
                    var result = await userManager.CreateAsync(player, "SenhaForte@123");

                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(player, Roles.Player.ToString());
                    else
                        _logger.LogWarning("Falha ao criar usuário {Email}: {Errors}", player.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            _logger.LogInformation("Seed de User.Player finalizada");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no Seed de User.Player");
        }
    }
    #endregion

    #region Default Data
    private async Task<List<Game>> SeedGamesAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Seed de Games iniciada");

            var gamesList = GetPreconfiguredGames();
            var savedGames = new List<Game>();

            foreach (var game in gamesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    return savedGames;

                var exists = await _gameRepository.ExistsAsync(game.EAN);

                if (!exists)
                {
                    await _gameRepository.AddAsync(game);
                    savedGames.Add(game);
                }
            }

            _logger.LogInformation("Seed de Games concluída");
            return savedGames;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no Seed de Games");
            return new List<Game>();
        }
    }

    private async Task SeedGalleryGamesAsync(List<Game> games, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Seed de GalleryGames iniciada");

            var rng = new Random();

            foreach (var game in games)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var exists = await _galleryRepository.OwnsGalleryGameAsync(game.EAN);

                if (!exists)
                {
                    var price = rng.Next(80, 300); // Preço aleatório
                    var galleryGame = new GalleryGame(game, price);

                    // Aplicar promoção aleatória
                    var promoType = rng.Next(0, 2) == 0 ? PromotionType.FixedDiscount : PromotionType.PercentageDiscount;
                    var discount = promoType == PromotionType.FixedDiscount ? 50m : rng.Next(10, 50);
                    var promotion = Promotion.Create(promoType, discount, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));
                    galleryGame.ApplyPromotion(promotion);

                    await _galleryRepository.AddToGalleryGameAsync(galleryGame);
                }
            }

            _logger.LogInformation("Seed de GalleryGames concluída");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no Seed de GalleryGames");
        }
    }

    private async Task SeedPlayersAsync(IServiceProvider sp, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Seed de Players iniciada");

            // 🔹 Cria um escopo separado para o contexto Identity
            using var identityScope = sp.CreateScope();
            var identityProvider = identityScope.ServiceProvider;
            var userManager = identityProvider.GetRequiredService<UserManager<User>>();

            // 🔹 O repositório de player continua vindo do escopo da aplicação
            var playerRepository = sp.GetRequiredService<IPlayerRepository>();

            foreach (var userData in GetPreconfiguredPlayerUsers())
            {
                if (cancellationToken.IsCancellationRequested) return;

                // Agora o userManager usa o contexto correto
                var dbUser = await userManager.FindByEmailAsync(userData.Email!);
                if (dbUser == null)
                {
                    _logger.LogWarning("Usuário {Email} não encontrado ao criar Player.", userData.Email);
                    continue;
                }

                var existingPlayer = await playerRepository.GetByUserIdAsync(dbUser.Id);
                if (existingPlayer == null)
                {
                    var player = new Player(dbUser.Id, userData.DisplayName!);
                    await playerRepository.AddAsync(player);
                    _logger.LogInformation("Player criado para usuário {Email}.", userData.Email);
                }
            }

            _logger.LogInformation("Seed de Players concluída");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no Seed de Player");
        }
    }


    private async Task SeedCartsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Seed de Carts iniciada");

            var players = (await _playerRepository.GetAllAsync()).Take(3).ToList();
            var galleryGames = await _galleryRepository.GetAllGalleryGamesAsync();

            foreach (var player in players)
            {
                if (cancellationToken.IsCancellationRequested) return;
                var cart = new Cart(player.Id);
                var playerGames = galleryGames.OrderBy(_ => Guid.NewGuid()).Take(2).ToList();
                foreach (var game in playerGames)
                {
                    cart.AddItem(game.Game);
                }
                await _cartRepository.AddAsync(cart);
            }

            _logger.LogInformation("Seed de Carts concluída");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no Seed de Cart");
        }
    }

    private async Task SeedLibraryAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Seed de LibraryGames iniciada");

            var players = (await _playerRepository.GetAllAsync()).Take(2).ToList();
            var galleryGames = await _galleryRepository.GetAllGalleryGamesAsync();

            foreach (var player in players)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var playerGames = galleryGames.OrderBy(_ => Guid.NewGuid()).Take(3).ToList();

                foreach (var game in playerGames)
                {
                    var ownsGameAsync = await _libraryRepository.OwnsLibraryGameAsync(player.Id, game.Id);

                    if (!ownsGameAsync)
                    {
                        var libraryGame = new LibraryGame(game.Game, player, game.FinalPrice);
                        await _libraryRepository.AddToLibraryAsync(libraryGame);
                    }
                }
            }

            _logger.LogInformation("Seed de LibraryGames concluída");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no Seed de LibraryGames");
        }
    }
#endregion

    #region private lists
    // Métodos de configuração de usuários omitidos para brevidade
    private List<User> GetPreconfiguredAdminUsers()
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
            }
        };
    }

    private List<User> GetPreconfiguredPlayerUsers()
    {
        return new List<User>
        {
            new User
            {
                FirstName = "Modesto",
                LastName = "Da Silva",
                DisplayName = "Modesto Da Silva",
                UserName = "modesto.silva",
                Email = "modesto.silva@emailbala.com.br"
            },
            new User
            {
                FirstName = "Jhon",
                LastName = "Lemon Lima",
                DisplayName = "Jhon Lemon Lima",
                UserName = "jhon.lemon",
                Email = "jhon.lemon@fugaz.com.br"
            },
            new User
            {
                FirstName = "Carla",
                LastName = "Monteiro",
                DisplayName = "Carla Monteiro",
                UserName = "carla.monteiro",
                Email = "carla.monteiro@techmail.com.br"
            },
            new User
            {
                FirstName = "Eduardo",
                LastName = "Ferraz Neto",
                DisplayName = "Eduardo Ferraz Neto",
                UserName = "eduardo.ferraz",
                Email = "eduardo.ferraz@inovatec.com"
            },
            new User
            {
                FirstName = "Luciana",
                LastName = "Campos Rocha",
                DisplayName = "Luciana Campos Rocha",
                UserName = "luciana.campos",
                Email = "luciana.campos@webplus.com.br"
            },
            new User
            {
                FirstName = "Rafael",
                LastName = "Almeida Costa",
                DisplayName = "Rafael Almeida Costa",
                UserName = "rafael.costa",
                Email = "rafael.costa@digitalhub.com"
            },
            new User
            {
                FirstName = "Tatiane",
                LastName = "Souza Lima",
                DisplayName = "Tatiane Souza Lima",
                UserName = "tatiane.lima",
                Email = "tatiane.lima@cloudmail.com"
            }
        };
    }

    private List<Game> GetPreconfiguredGames()
    {
        return new List<Game>   
            {
                new Game("7891234560001", "Cyber Rebellion", "Action", "Lute contra corporações em um futuro distópico"),
                new Game("7891234560002", "Ocean's Whisper", "Adventure", "Explore os mistérios das profundezas do oceano"),
                new Game("7891234560003", "Pixel Brawlers", "Fighting", "Batalhas frenéticas em estilo retrô"),
                new Game("7891234560004", "Sky Realms", "RPG", "Aventure-se por reinos flutuantes mágicos"),
                new Game("7891234560005", "Turbo Drift X", "Racing", "Corridas urbanas com drift extremo"),
                new Game("7891234560006", "Battlefield Echo", "Shooter", "Combates táticos em zonas de guerra modernas"),
                new Game("7891234560007", "Haunted Carnival", "Horror", "Sobreviva a um parque de diversões amaldiçoado"),
                new Game("7891234560008", "Chef's Empire", "Simulation", "Construa seu império gastronômico"),
                new Game("7891234560009", "Alien Harvest", "Strategy", "Gerencie recursos em um planeta alienígena"),
                new Game("7891234560010", "Legends of Valoria", "RPG", "Heróis lendários em batalhas épicas"),
                new Game("7891234560011", "Neon Skater", "Sports", "Skate futurista em pistas iluminadas por neon"),
                new Game("7891234560012", "Witchlight Academy", "Adventure", "Descubra segredos mágicos em uma escola encantada"),
                new Game("7891234560013", "Titan Clash", "Fighting", "Lute como titãs em arenas colossais"),
                new Game("7891234560014", "Quantum Heist", "Action", "Roube segredos temporais em missões furtivas"),
                new Game("7891234560015", "Farmtopia", "Simulation", "Cultive, crie animais e expanda sua fazenda"),
                new Game("7891234560016", "Dark Horizon", "Horror", "Terror psicológico em uma estação espacial"),
                new Game("7891234560017", "Kingdoms Unite", "Strategy", "Unifique reinos e conquiste territórios"),
                new Game("7891234560018", "Robo Soccer League", "Sports", "Futebol com robôs em arenas futuristas"),
                new Game("7891234560019", "Shadow Operatives", "Shooter", "Operações secretas em zonas de conflito"),
                new Game("7891234560020", "Crystal Caverns", "Adventure", "Explore cavernas cheias de enigmas e tesouros"),
                new Game("7891234560021", "Monster Chef", "Simulation", "Cozinhe pratos exóticos para monstros famintos"),
                new Game("7891234560022", "Galactic Traders", "Strategy", "Negocie recursos entre planetas e impérios"),
                new Game("7891234560023", "Viking Saga", "RPG", "Viva como um guerreiro nórdico em busca de glória"),
                new Game("7891234560024", "Urban Drift", "Racing", "Desafie as ruas da cidade com carros tunados"),
                new Game("7891234560025", "Samurai Reborn", "Action", "Honra e vingança no Japão feudal"),
                new Game("7891234560026", "Echoes of the Past", "Adventure", "Viaje no tempo para corrigir eventos históricos"),
                new Game("7891234560027", "Zombie Kart", "Racing", "Corridas malucas com zumbis e armadilhas"),
                new Game("7891234560028", "Dungeon Architects", "Simulation", "Construa e defenda sua masmorra"),
                new Game("7891234560029", "Sky Pirates", "Action", "Batalhas aéreas entre piratas voadores"),
                new Game("7891234560030", "Mythos Arena", "Fighting", "Deuses e monstros em combates lendários"),
                new Game("7891234560031", "Tactical Minds", "Strategy", "Xadrez moderno com unidades militares"),
                new Game("7891234560032", "Haiku Spirits", "Adventure", "Uma jornada poética por mundos espirituais"),
                new Game("7891234560033", "Inferno Run", "Action", "Escape de um mundo em chamas"),
                new Game("7891234560034", "Retro Racers", "Racing", "Corridas arcade com visual dos anos 80"),
                new Game("7891234560035", "Beast Tamer", "RPG", "Domine e treine criaturas mágicas"),
                new Game("7891234560036", "Virtual Chef VR", "Simulation", "Cozinha imersiva em realidade virtual"),
                new Game("7891234560037", "Alien Panic", "Shooter", "Defenda a Terra de uma invasão alienígena"),
                new Game("7891234560038", "Frozen Realms", "Adventure", "Desvende os mistérios de um mundo congelado"),
                new Game("7891234560039", "Cyber Arena", "Fighting", "Combates cibernéticos em arenas digitais"),
                new Game("7891234560040", "World Rally X", "Racing", "Ralis extremos em pistas ao redor do mundo"),
            };
    }
    #endregion
}
