using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces;
using FCG.Domain.Data.Contexts;
using FCG.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FCG.Application.Services;

public class PlayerService : IPlayerService
{
    private readonly FcgDbContext _context;
    private readonly UserDbContext _userContext;

    public PlayerService(FcgDbContext context, UserDbContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<PlayerWithUserResponseDto?> GetByIdAsync(Guid playerId)
    {
        var player = await _context.Players
            .Include(p => p.Library)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return null;

        var user = await _userContext.Users.FindAsync(player.UserId);
        if (user == null) return null;

        return new PlayerWithUserResponseDto
        {
            PlayerId = player.Id,
            DisplayName = player.DisplayName,
            Games = player.Library.Select(g => g.Name).ToList(),
            UserId = user.Id,
            UserFullName = user.FullName,
            Email = user.Email ?? string.Empty
        };
    }

    public async Task<OperationResult> CreateAsync(PlayerCreateDto dto)
    {
        var exists = await _context.Players.AnyAsync(p => p.UserId == dto.UserId);
        if (exists)
            return OperationResult.Failure("Jogador já existe para este usuário.");

        var player = new Player(dto.UserId, dto.DisplayName);
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateDisplayNameAsync(Guid playerId, string newDisplayName)
    {
        var player = await _context.Players.FindAsync(playerId);
        if (player == null)
            return OperationResult.Failure("Jogador não encontrado.");

        player.AlterarDisplayName(newDisplayName);
        await _context.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task<OperationResult> AddGameToLibraryAsync(Guid playerId, Guid gameId)
    {
        var player = await _context.Players
            .Include(p => p.Library)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null)
            return OperationResult.Failure("Jogador não encontrado.");

        var game = await _context.Games.FindAsync(gameId);
        if (game == null)
            return OperationResult.Failure("Jogo não encontrado.");

        player.AdicionarJogo(game);
        await _context.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task<IEnumerable<PlayerResponseDto>> GetAllAsync()
    {
        var players = await _context.Players
            .Include(p => p.Library)
            .ToListAsync();

        return players.Select(p => new PlayerResponseDto
        {
            Id = p.Id,
            DisplayName = p.DisplayName,
            Games = p.Library.Select(g => g.Name).ToList()
        });
    }

    public async Task<IEnumerable<GameRequestDto>> GetPurchasedGamesAsync(Guid playerId)
    {
        var purchases = await _context.Purchases
            .Include(p => p.Game)
            .Where(p => p.PlayerId == playerId)
            .ToListAsync();

        return purchases.Select(p => new GameRequestDto
        {
            Id = p.Game.Id,
            Name = p.Game.Name,
            Genre = p.Game.Genre,
            Description = p.Game.Description,
            Price = p.Game.PrecoFinal
        });
    }

    public async Task<IEnumerable<GameRequestDto>> GetCartItemsAsync(Guid playerId)
    {
        var cartItems = await _context.CartItems
            .Include(c => c.Game)
            .Where(c => c.PlayerId == playerId)
            .ToListAsync();

        return cartItems.Select(c => new GameRequestDto
        {
            Id = c.Game.Id,
            Name = c.Game.Name,
            Genre = c.Game.Genre,
            Description = c.Game.Description,
            Price = c.Game.PrecoFinal
        });
    }

    public async Task<IEnumerable<GameListResponseDto>> GetAvailableGamesAsync(
        string? orderBy,
        bool excludeOwned,
        Guid? playerId)
    {
        var query = _context.Games.AsQueryable();

        if (excludeOwned && playerId.HasValue)
        {
            var ownedGameIds = await _context.Purchases
                .Where(p => p.PlayerId == playerId.Value)
                .Select(p => p.GameId)
                .ToListAsync();

            query = query.Where(g => !ownedGameIds.Contains(g.Id));
        }

        query = orderBy switch
        {
            "discount-fixed-desc" => query.OrderByDescending(g =>
                g.Promotion.Type == PromotionType.FixedDiscount ? g.Promotion.Value : 0),

            "discount-fixed-asc" => query.OrderBy(g =>
                g.Promotion.Type == PromotionType.FixedDiscount ? g.Promotion.Value : 0),

            "discount-percent-desc" => query.OrderByDescending(g =>
                g.Promotion.Type == PromotionType.PercentageDiscount ? g.Promotion.Value : 0),

            "discount-percent-asc" => query.OrderBy(g =>
                g.Promotion.Type == PromotionType.PercentageDiscount ? g.Promotion.Value : 0),

            _ => query.OrderBy(g => g.Name)
        };

        var games = await query.ToListAsync();

        return games.Select(g => new GameListResponseDto
        {
            Id = g.Id,
            Name = g.Name,
            Genre = g.Genre,
            Description = g.Description,
            PrecoFinal = g.PrecoFinal,
            EmPromocao = g.Promotion.IsActive(DateTime.UtcNow),
            TipoPromocao = g.Promotion.Type.ToString(),
            ValorPromocao = g.Promotion.Value
        });
    }
}