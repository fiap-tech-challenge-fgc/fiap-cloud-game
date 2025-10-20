using FCG.Application.Dtos;
using FCG.Application.Dtos.Response;
using FCG.Application.Interfaces.Service;
using FCG.Domain.Data.Contexts;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Application.Services;

public class GameService : IGameService
{
    private readonly FcgDbContext _context;

    public GameService(FcgDbContext context)
    {
        _context = context;
    }

    public async Task<GameResponseDto> CreateGameAsync(GameCreateDto dto)
    {
        var game = new Game(dto.Name, dto.Genre, dto.Description, dto.Price);
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return MapToResponse(game);
    }

    public async Task<GameResponseDto?> GetGameByIdAsync(Guid id)
    {
        var game = await _context.Games.FindAsync(id);
        return game == null ? null : MapToResponse(game);
    }

    public async Task<IEnumerable<GameResponseDto>> GetAllGamesAsync()
    {
        var games = await _context.Games.ToListAsync();
        return games.Select(MapToResponse);
    }

    public async Task<OperationResult> DeleteGameAsync(Guid id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game == null)
            return OperationResult.Failure("Game não encontrado.");

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();

        return OperationResult.Success();
    }

    private GameResponseDto MapToResponse(Game game)
    {
        return new GameResponseDto
        {
            Id = game.Id,
            Name = game.Name,
            Genre = game.Genre,
            Description = game.Description,
            Price = game.Price,
            FinalPrice = game.PrecoFinal,
            PromotionDescription = game.Promotion.IsActive(DateTime.UtcNow)
                ? $"{game.Promotion.Type} - {game.Promotion.Value}"
                : "Sem promoção"
        };
    }
}