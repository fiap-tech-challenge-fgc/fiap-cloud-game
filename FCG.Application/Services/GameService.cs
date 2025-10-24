using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Result;
using FCG.Domain.Data.Contexts;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FCG.Application.Services;

public class GameService : IGameService
{
    private readonly FcgDbContext _context;
    private readonly ILogger<GameService> _logger;

    public GameService(FcgDbContext context, ILogger<GameService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OperationResult<GameResponseDto>> CreateGameAsync(GameCreateRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return OperationResult<GameResponseDto>.Failure("O nome do jogo é obrigatório.");
        if (dto.Price <= 0)
            return OperationResult<GameResponseDto>.Failure("O preço deve ser maior que zero.");

        var game = new Game(dto.Name, dto.Genre, dto.Description, dto.Price);
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Jogo criado com sucesso: {GameId}", game.Id);

        return OperationResult<GameResponseDto>.Success(MapToResponse(game));
    }

    public async Task<GameResponseDto?> GetGameByIdAsync(Guid id)
    {
        var game = await _context.Games
            .Include(g => g.Promotion)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null)
        {
            _logger.LogWarning("Jogo não encontrado: {GameId}", id);
            return null;
        }

        return MapToResponse(game);
    }


    public async Task<PagedResult<GameResponseDto>> GetAllGamesAsync(PagedRequestDto<GameFilterDto, GameOrderDto> pagedRequestDto)
    {
        var query = _context.Games.Include(g => g.Promotion).AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(pagedRequestDto.Filter?.Name))
            query = query.Where(g => g.Name.Contains(pagedRequestDto.Filter.Name));

        if (!string.IsNullOrWhiteSpace(pagedRequestDto.Filter?.Genre))
            query = query.Where(g => g.Genre.Contains(pagedRequestDto.Filter.Genre));

        if (pagedRequestDto.Filter?.MinPrice.HasValue == true)
            query = query.Where(g => g.Price >= pagedRequestDto.Filter.MinPrice.Value);

        if (pagedRequestDto.Filter?.MaxPrice.HasValue == true)
            query = query.Where(g => g.Price <= pagedRequestDto.Filter.MaxPrice.Value);


        string? orderBy = null;
        bool ascending = true;

        if (pagedRequestDto.OrderBy is { } order)
        {
            orderBy = order.OrderBy?.ToLower();
            ascending = order.Ascending;
        }

        query = orderBy switch
        {
            "name" => ascending ? query.OrderBy(g => g.Name) : query.OrderByDescending(g => g.Name),
            "price" => ascending ? query.OrderBy(g => g.Price) : query.OrderByDescending(g => g.Price),
            _ => query.OrderBy(g => g.Name) // padrão
        };

        var totalItems = await query.CountAsync();

        var games = await query
            .Skip((pagedRequestDto.PageNumber - 1) * pagedRequestDto.PageSize)
            .Take(pagedRequestDto.PageSize)
            .ToListAsync();

        return new PagedResult<GameResponseDto>
        {
            Items = games.Select(MapToResponse),
            PageNumber = pagedRequestDto.PageNumber,
            PageSize = pagedRequestDto.PageSize,
            TotalItems = totalItems
        };
    }


    public async Task<OperationResult> DeleteGameAsync(Guid id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game == null)
        {
            _logger.LogWarning("Tentativa de exclusão falhou. Jogo não encontrado: {GameId}", id);
            return OperationResult.Failure("Game não encontrado.");
        }

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Jogo excluído com sucesso: {GameId}", id);

        return OperationResult<GameResponseDto>.Success(null);
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
            PromotionDescription = game.Promotion != null && game.Promotion.IsActive(DateTime.UtcNow)
                ? $"{game.Promotion.Type} - {game.Promotion.Value}"
                : "Sem promoção"
        };
    }
}