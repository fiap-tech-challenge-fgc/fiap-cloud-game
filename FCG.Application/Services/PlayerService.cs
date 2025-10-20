using FCG.Application.Dtos;
using FCG.Application.Dtos.Response;
using FCG.Application.Interfaces;
using FCG.Domain.Data.Contexts;
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

    public async Task<PlayerWithUserDto?> GetByIdAsync(Guid playerId)
    {
        var player = await _context.Players
            .Include(p => p.Library)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return null;

        var user = await _userContext.Users.FindAsync(player.UserId);
        if (user == null) return null;

        return new PlayerWithUserDto
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
}