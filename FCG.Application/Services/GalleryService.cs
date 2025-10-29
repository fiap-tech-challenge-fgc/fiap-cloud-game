using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Repository;
using FCG.Application.Interfaces.Service;
using FCG.Application.Repositories;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using FCG.Domain.ValuesObjects;
using Microsoft.Extensions.Logging;

namespace FCG.Application.Services;

public class GalleryService : IGalleryService
{
    private readonly IGalleryRepository _galleryRepository;
    private readonly ILogger<GalleryService> _logger;

    public GalleryService(ILogger<GalleryService> logger, IGalleryRepository galleryRepository)
    {
        _logger = logger;
        _galleryRepository = galleryRepository;
    }

    public async Task<OperationResult<GalleryGameResponseDto>> AddToGalleryAsync(GalleryGameCreateRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EAN))
            return OperationResult<GalleryGameResponseDto>.Failure("O EAN do jogo é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return OperationResult<GalleryGameResponseDto>.Failure("O nome do jogo é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Genre))
            return OperationResult<GalleryGameResponseDto>.Failure("O gênero do jogo é obrigatório.");

        if (dto.Price <= 0)
            return OperationResult<GalleryGameResponseDto>.Failure("O preço deve ser maior que zero.");

        var exist = await _galleryRepository.ExistsAsync(dto.EAN);

        if (exist)
        {
            return OperationResult<GalleryGameResponseDto>.Failure("O EAN do jogo já existe na galleria, utilize a editação.");
        }

        var game = new GalleryGame(dto.EAN, dto.Name, dto.Genre, dto.Description, dto.Price);
        await _galleryRepository.AddToGalleryAsync(game);

        _logger.LogInformation("Jogo adicionado à galeria com sucesso: {GameId}", game.Id);

        return OperationResult<GalleryGameResponseDto>.Success(MapToResponse(game));
    }

    public async Task<OperationResult<GalleryGameResponseDto>> UpdateGalleryGameAsync(Guid id, GalleryGameCreateRequestDto dto)
    {
        var existingGame = await _galleryRepository.GetGalleryGameByIdAsync(id);
        if (existingGame == null)
            return OperationResult<GalleryGameResponseDto>.Failure("Jogo não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.EAN))
            return OperationResult<GalleryGameResponseDto>.Failure("O EAN do jogo é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return OperationResult<GalleryGameResponseDto>.Failure("O nome do jogo é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Genre))
            return OperationResult<GalleryGameResponseDto>.Failure("O gênero do jogo é obrigatório.");

        if (dto.Price <= 0)
            return OperationResult<GalleryGameResponseDto>.Failure("O preço deve ser maior que zero.");

        // Update properties while keeping the same ID
        var game = new GalleryGame(dto.EAN, dto.Name, dto.Genre, dto.Description, dto.Price);
        await _galleryRepository.UpdateGalleryGameAsync(game);

        _logger.LogInformation("Jogo atualizado na galeria com sucesso: {GameId}", game.Id);

        return OperationResult<GalleryGameResponseDto>.Success(MapToResponse(game));
    }

    public async Task<OperationResult> RemoveFromGalleryAsync(Guid id)
    {
        var game = await _galleryRepository.GetGalleryGameByIdAsync(id);
        if (game == null)
            return OperationResult.Failure("Jogo não encontrado.");

        await _galleryRepository.RemoveFromGalleryAsync(game);
        _logger.LogInformation("Jogo removido da galeria com sucesso: {GameId}", id);

        return OperationResult.Success();
    }

    public async Task<OperationResult> ApplyPromotionAsync(Guid id, string promotionType, decimal value, DateTime? startDate = null, DateTime? endDate = null)
    {
        var game = await _galleryRepository.GetGalleryGameByIdAsync(id);
        if (game == null)
            return OperationResult.Failure("Jogo não encontrado.");

        if (value <= 0)
            return OperationResult.Failure("O valor da promoção deve ser maior que zero.");

        if (!Enum.TryParse<PromotionType>(promotionType, true, out var type))
            return OperationResult.Failure("Tipo de promoção inválido. Tipos válidos: FixedDiscount, PercentageDiscount");

        var start = startDate ?? DateTime.UtcNow;
        var end = endDate ?? start.AddMonths(1);

        var promotion = Promotion.Create(type, value, start, end);
        game.ApplyPromotion(promotion);

        await _galleryRepository.UpdateGalleryGameAsync(game);
        _logger.LogInformation("Promoção aplicada com sucesso: {GameId}", id);

        return OperationResult.Success();
    }

    public async Task<OperationResult> RemovePromotionAsync(Guid id)
    {
        var game = await _galleryRepository.GetGalleryGameByIdAsync(id);
        if (game == null)
            return OperationResult.Failure("Jogo não encontrado.");

        game.ApplyPromotion(Promotion.None);
        await _galleryRepository.UpdateGalleryGameAsync(game);

        _logger.LogInformation("Promoção removida com sucesso: {GameId}", id);

        return OperationResult.Success();
    }

    public async Task<OperationResult<GalleryGameResponseDto>> GetGalleryGameByIdAsync(Guid id)
    {
        var game = await _galleryRepository.GetGalleryGameByIdAsync(id);
        if (game == null)
            return OperationResult<GalleryGameResponseDto>.Failure("Jogo não encontrado.");

        return OperationResult<GalleryGameResponseDto>.Success(MapToResponse(game));
    }

    public async Task<OperationResult<PagedResult<GalleryGameResponseDto>>> GetGalleryGamesAsync(PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        try
        {
            var games = (await _galleryRepository.GetAllGalleryGamesAsync()).AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(dto.Filter?.Name))
                games = games.Where(g => g.Name.Contains(dto.Filter.Name));

            if (!string.IsNullOrWhiteSpace(dto.Filter?.Genre))
                games = games.Where(g => g.Genre.Contains(dto.Filter.Genre));

            if (dto.Filter?.MinPrice.HasValue == true)
                games = games.Where(g => g.Price >= dto.Filter.MinPrice.Value);

            if (dto.Filter?.MaxPrice.HasValue == true)
                games = games.Where(g => g.Price <= dto.Filter.MaxPrice.Value);

            // Apply ordering
            string? orderBy = dto.OrderBy?.OrderBy?.ToLower();
            bool ascending = dto.OrderBy?.Ascending ?? true;

            games = orderBy switch
            {
                "name" => ascending ? games.OrderBy(g => g.Name) : games.OrderByDescending(g => g.Name),
                "price" => ascending ? games.OrderBy(g => g.Price) : games.OrderByDescending(g => g.Price),
                "genre" => ascending ? games.OrderBy(g => g.Genre) : games.OrderByDescending(g => g.Genre),
                _ => games.OrderBy(g => g.Name)
            };

            // Apply pagination
            var totalItems = games.Count();
            var pagedGames = games
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToList();

            var resultMap = new PagedResult<GalleryGameResponseDto>
            {
                Items = pagedGames.Select(MapToResponse),
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                TotalItems = totalItems
            };

            return OperationResult<PagedResult<GalleryGameResponseDto>>.Success(resultMap);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter a galeria de jogos.");
            return OperationResult<PagedResult<GalleryGameResponseDto>>.Failure("Erro ao obter a galeria de jogos.");
        }
    }

    public async Task<OperationResult<IEnumerable<GalleryGameResponseDto>>> GetPromotionalGamesAsync()
    {
        try
        {
            var games = await _galleryRepository.GetPromotionalGamesAsync();
            return OperationResult<IEnumerable<GalleryGameResponseDto>>.Success(games.Select(MapToResponse));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter os jogos em promoção.");
            return OperationResult<IEnumerable<GalleryGameResponseDto>>.Failure("Erro ao obter os jogos em promoção.");
        }
    }

    public async Task<OperationResult<bool>> IsGameAvailableForPurchaseAsync(Guid id)
    {
        return OperationResult<bool>.Success(await _galleryRepository.IsAvailableForPurchaseAsync(id));
    }

    public async Task<OperationResult<decimal>> GetGamePriceAsync(Guid id)
    {
        var game = await _galleryRepository.GetGalleryGameByIdAsync(id);
        return OperationResult<decimal>.Success(game?.FinalPrice ?? 0);
    }

    public async Task<OperationResult<PagedResult<GalleryGameResponseDto>>> GetAvailableGamesAsync(PagedRequestDto<GameFilterDto, GameOrderDto> dto, Guid playerId)
    {
        var games = (await _galleryRepository.GetAllGalleryGamesAsync()).AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(dto.Filter?.Name))
            games = games.Where(g => g.Name.Contains(dto.Filter.Name));

        if (!string.IsNullOrWhiteSpace(dto.Filter?.Genre))
            games = games.Where(g => g.Genre.Contains(dto.Filter.Genre));

        if (dto.Filter?.MinPrice.HasValue == true)
            games = games.Where(g => g.Price >= dto.Filter.MinPrice.Value);

        if (dto.Filter?.MaxPrice.HasValue == true)
            games = games.Where(g => g.Price <= dto.Filter.MaxPrice.Value);

        // Apply ordering
        string? orderBy = dto.OrderBy?.OrderBy?.ToLower();
        bool ascending = dto.OrderBy?.Ascending ?? true;

        games = orderBy switch
        {
            "name" => ascending ? games.OrderBy(g => g.Name) : games.OrderByDescending(g => g.Name),
            "price" => ascending ? games.OrderBy(g => g.Price) : games.OrderByDescending(g => g.Price),
            "genre" => ascending ? games.OrderBy(g => g.Genre) : games.OrderByDescending(g => g.Genre),
            _ => games.OrderBy(g => g.Name)
        };

        // Apply pagination
        var totalItems = games.Count();
        var pagedGames = games
            .Skip((dto.PageNumber - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToList();


        var pagedResult = new PagedResult<GalleryGameResponseDto>
        {
            Items = pagedGames.Select(MapToResponse),
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = totalItems
        };

        return OperationResult<PagedResult<GalleryGameResponseDto>>.Success(pagedResult);
    }

    private GalleryGameResponseDto MapToResponse(GalleryGame game)
    {
        return new GalleryGameResponseDto
        {
            Id = game.Id,
            EAN = game.EAN,
            Name = game.Name,
            Genre = game.Genre,
            Description = game.Description,
            Price = game.Price,
            FinalPrice = game.FinalPrice,
            PromotionDescription = game.Promotion != null && game.Promotion.IsActive(DateTime.UtcNow)
                ? $"{game.Promotion.Type.ToString()} - {game.Promotion.Value}"
                : "Sem promoção",
            OnSale = game.Promotion != null && game.Promotion.IsActive(DateTime.UtcNow),
            TypePromotion = game.Promotion?.Type.ToString() ?? string.Empty,
            PromotionValue = game.Promotion?.Value ?? 0
        };
    }
}