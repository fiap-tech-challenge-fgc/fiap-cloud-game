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
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<GalleryService> _logger;

    public GalleryService(ILogger<GalleryService> logger, IGalleryRepository galleryRepository, IGameRepository gameRepository)
    {
        _logger = logger;
        _galleryRepository = galleryRepository;
        _gameRepository = gameRepository;
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

        var exist = await _galleryRepository.OwnsGalleryGameAsync(dto.EAN);

        if (exist)
        {
            return OperationResult<GalleryGameResponseDto>.Failure("O EAN do jogo já existe na galleria, utilize a editação.");
        }

        var game = await _gameRepository.GetByIdAsync(dto.EAN);

        if (game == null)
            return OperationResult<GalleryGameResponseDto>.Failure("Game não encontrado pelo EAN.");

        var galleryGame = new GalleryGame(game, dto.Price);

        await _galleryRepository.AddToGalleryGameAsync(galleryGame);

        _logger.LogInformation("Jogo adicionado à galeria com sucesso: {GameId}", game.Id);

        return OperationResult<GalleryGameResponseDto>.Success(MapToResponse(galleryGame));
    }

    public async Task<OperationResult<GalleryGameResponseDto>> UpdateGalleryGameAsync(Guid id, GalleryGameCreateRequestDto dto)
    {
        var existingGame = await _galleryRepository.OwnsGalleryGameAsync(id);

        if (!existingGame)
            return OperationResult<GalleryGameResponseDto>.Failure("Jogo não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.EAN))
            return OperationResult<GalleryGameResponseDto>.Failure("O EAN do jogo é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return OperationResult<GalleryGameResponseDto>.Failure("O nome do jogo é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Genre))
            return OperationResult<GalleryGameResponseDto>.Failure("O gênero do jogo é obrigatório.");

        if (dto.Price <= 0)
            return OperationResult<GalleryGameResponseDto>.Failure("O preço deve ser maior que zero.");

        var game = await _gameRepository.GetByIdAsync(dto.EAN);

        if (game == null)
            return OperationResult<GalleryGameResponseDto>.Failure("Game não encontrado pelo EAN.");

        var galleryGame = new GalleryGame(game, dto.Price);

        // Update properties while keeping the same ID
        await _galleryRepository.UpdateGalleryGameAsync(galleryGame);

        _logger.LogInformation("Jogo atualizado na galeria com sucesso: {GameId}", game.Id);

        return OperationResult<GalleryGameResponseDto>.Success(MapToResponse(galleryGame));
    }

    public async Task<OperationResult> RemoveFromGalleryAsync(Guid id)
    {
        var galleryGames = await _galleryRepository.GetGalleryGameByIdAsync(id);
        if (galleryGames == null)
            return OperationResult.Failure("Jogo não encontrado.");

        await _galleryRepository.RemoveFromGalleryGameAsync(galleryGames);
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
        var galleryGames = await _galleryRepository.GetGalleryGameByIdAsync(id);
        if (galleryGames == null)
            return OperationResult.Failure("Jogo não encontrado.");

        galleryGames.ApplyPromotion(Promotion.None);
        await _galleryRepository.UpdateGalleryGameAsync(galleryGames);

        _logger.LogInformation("Promoção removida com sucesso: {GameId}", id);

        return OperationResult.Success();
    }

    public async Task<OperationResult<GalleryGameResponseDto>> GetGalleryGameByIdAsync(Guid id)
    {
        var galleryGame = await _galleryRepository.GetGalleryGameByIdAsync(id);
        if (galleryGame == null)
            return OperationResult<GalleryGameResponseDto>.Failure("Jogo não encontrado.");

        return OperationResult<GalleryGameResponseDto>.Success(MapToResponse(galleryGame));
    }

    public async Task<OperationResult<PagedResult<GalleryGameResponseDto>>> GetGalleryGamesAsync(PagedRequestDto<GameFilterDto, GameOrderDto> dto)
    {
        try
        {
            var galleryGames = (await _galleryRepository.GetAllGalleryGamesAsync()).AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(dto.Filter?.Name))
                galleryGames = galleryGames.Where(g => g.Game.Name.Contains(dto.Filter.Name));

            if (!string.IsNullOrWhiteSpace(dto.Filter?.Genre))
                galleryGames = galleryGames.Where(g => g.Game.Genre.Contains(dto.Filter.Genre));

            if (dto.Filter?.MinPrice.HasValue == true)
                galleryGames = galleryGames.Where(g => g.Price >= dto.Filter.MinPrice.Value);

            if (dto.Filter?.MaxPrice.HasValue == true)
                galleryGames = galleryGames.Where(g => g.Price <= dto.Filter.MaxPrice.Value);

            // Apply ordering
            string? orderBy = dto.OrderBy?.OrderBy?.ToLower();
            bool ascending = dto.OrderBy?.Ascending ?? true;

            galleryGames = orderBy switch
            {
                "name" => ascending ? galleryGames.OrderBy(g =>  g.Game.Name) : galleryGames.OrderByDescending(g =>  g.Game.Name),
                "genre" => ascending ? galleryGames.OrderBy(g => g.Game.Genre) : galleryGames.OrderByDescending(g => g.Game.Genre),
                "price" => ascending ? galleryGames.OrderBy(g => g.Price) : galleryGames.OrderByDescending(g => g.Price),
                _ => galleryGames.OrderBy(g => g.Game.Name)
            };

            // Apply pagination
            var totalItems = galleryGames.Count();
            var pagedGames = galleryGames
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
            var galleryGames = await _galleryRepository.GetPromotionalGalleryGameAsync();
            return OperationResult<IEnumerable<GalleryGameResponseDto>>.Success(galleryGames.Select(MapToResponse));
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
        var galleryGame = await _galleryRepository.GetGalleryGameByIdAsync(id);
        return OperationResult<decimal>.Success(galleryGame?.FinalPrice ?? 0);
    }

    public async Task<OperationResult<PagedResult<GalleryGameResponseDto>>> GetAvailableGamesAsync(PagedRequestDto<GameFilterDto, GameOrderDto> dto, Guid playerId)
    {
        var galleryGames = (await _galleryRepository.GetAllGalleryGamesAsync()).AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(dto.Filter?.Name))
            galleryGames = galleryGames.Where(g => g.Game.Name.Contains(dto.Filter.Name));

        if (!string.IsNullOrWhiteSpace(dto.Filter?.Genre))
            galleryGames = galleryGames.Where(g => g.Game.Genre.Contains(dto.Filter.Genre));

        if (dto.Filter?.MinPrice.HasValue == true)
            galleryGames = galleryGames.Where(g => g.Price >= dto.Filter.MinPrice.Value);

        if (dto.Filter?.MaxPrice.HasValue == true)
            galleryGames = galleryGames.Where(g => g.Price <= dto.Filter.MaxPrice.Value);

        // Apply ordering
        string? orderBy = dto.OrderBy?.OrderBy?.ToLower();
        bool ascending = dto.OrderBy?.Ascending ?? true;

        galleryGames = orderBy switch
        {
            "name" => ascending ? galleryGames.OrderBy(g =>  g.Game.Name) : galleryGames.OrderByDescending(g => g.Game.Name),
            "genre" => ascending ? galleryGames.OrderBy(g => g.Game.Genre) : galleryGames.OrderByDescending(g => g.Game.Genre),
            "price" => ascending ? galleryGames.OrderBy(g => g.Price) : galleryGames.OrderByDescending(g => g.Price),
            _ => galleryGames.OrderBy(g => g.Game.Name)
        };

        // Apply pagination
        var totalItems = galleryGames.Count();
        var pagedGames = galleryGames
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

    private GalleryGameResponseDto MapToResponse(GalleryGame gallery)
    {
        return new GalleryGameResponseDto
        {
            Id = gallery.Id,
            EAN = gallery.Game.EAN,
            Name = gallery.Game.Name,
            Genre = gallery.Game.Genre,
            Description = gallery.Game.Description,
            Price = gallery.Price,
            FinalPrice = gallery.FinalPrice,
            PromotionDescription = gallery.Promotion != null && gallery.Promotion.IsActive(DateTime.UtcNow)
                ? $"{gallery.Promotion.Type.ToString()} - {gallery.Promotion.Value}"
                : "Sem promoção",
            OnSale = gallery.Promotion != null && gallery.Promotion.IsActive(DateTime.UtcNow),
            TypePromotion = gallery.Promotion?.Type.ToString() ?? string.Empty,
            PromotionValue = gallery.Promotion?.Value ?? 0
        };
    }
}