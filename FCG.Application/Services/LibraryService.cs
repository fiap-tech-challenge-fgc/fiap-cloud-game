using FCG.Application.Dto.Filter;
using FCG.Application.Dto.Order;
using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Repository;
using FCG.Application.Interfaces.Service;
using FCG.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FCG.Application.Services;

public class LibraryService : ILibraryService
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IGalleryRepository _galleryRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(
        ILibraryRepository libraryRepository,
        IGalleryRepository galleryRepository,
        IPlayerRepository playerRepository,
        ILogger<LibraryService> logger)
    {
        _libraryRepository = libraryRepository;
        _galleryRepository = galleryRepository;
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<OperationResult<LibraryGameResponseDto>> AddToLibraryAsync(Guid playerId, Guid galleryGameId, decimal purchasePrice)
    {
        try
        {
            var galleryGame = await _galleryRepository.GetGalleryGameByIdAsync(galleryGameId);
            if (galleryGame == null)
                return OperationResult<LibraryGameResponseDto>.Failure("Jogo não encontrado na galeria.");

            if (await _libraryRepository.HasGameInLibraryAsync(playerId, galleryGame.Name))
                return OperationResult<LibraryGameResponseDto>.Failure("Jogador já possui este jogo.");

            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null)
                return OperationResult<LibraryGameResponseDto>.Failure("Jogador não encontrado.");

            var libraryGame = new LibraryGame(galleryGame, player, purchasePrice);
            await _libraryRepository.AddToLibraryAsync(libraryGame);

            _logger.LogInformation("Jogo adicionado à biblioteca: {GameId} para o jogador: {PlayerId}", galleryGameId, playerId);

            return OperationResult<LibraryGameResponseDto>.Success(MapToResponse(libraryGame));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar jogo à biblioteca: {GameId} para o jogador: {PlayerId}", galleryGameId, playerId);
            return OperationResult<LibraryGameResponseDto>.Failure("Erro ao adicionar jogo à biblioteca.");
        }
    }

    public async Task<OperationResult<LibraryGameResponseDto>> GetLibraryGameAsync(Guid playerId, Guid gameId)
    {
        try
        {
            var game = await _libraryRepository.GetLibraryGameAsync(playerId, gameId);
            if (game == null)
            {
                _logger.LogWarning("Jogo não encontrado na biblioteca: {GameId} para o jogador: {PlayerId}", gameId, playerId);
                return OperationResult<LibraryGameResponseDto>.Failure("Jogo não encontrado na biblioteca.");
            }

            return OperationResult<LibraryGameResponseDto>.Success(MapToResponse(game));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar jogo na biblioteca: {GameId} para o jogador: {PlayerId}", gameId, playerId);
            return OperationResult<LibraryGameResponseDto>.Failure("Erro ao buscar jogo na biblioteca.");
        }
    }

    public async Task<OperationResult<PagedResult<LibraryGameResponseDto>>> GetPlayerLibraryAsync(Guid playerId, PagedRequestDto<GameFilterDto, GameOrderDto> pagedRequestDto)
    {
        try
        {
            var games = (await _libraryRepository.GetPlayerLibraryAsync(playerId)).AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(pagedRequestDto.Filter?.Name))
                games = games.Where(g => g.Name.Contains(pagedRequestDto.Filter.Name));

            if (!string.IsNullOrWhiteSpace(pagedRequestDto.Filter?.Genre))
                games = games.Where(g => g.Genre.Contains(pagedRequestDto.Filter.Genre));

            // Apply ordering
            string? orderBy = pagedRequestDto.OrderBy?.OrderBy?.ToLower();
            bool ascending = pagedRequestDto.OrderBy?.Ascending ?? true;

            games = orderBy switch
            {
                "name" => ascending ? games.OrderBy(g => g.Name) : games.OrderByDescending(g => g.Name),
                "genre" => ascending ? games.OrderBy(g => g.Genre) : games.OrderByDescending(g => g.Genre),
                "purchasedate" => ascending ? games.OrderBy(g => g.PurchaseDate) : games.OrderByDescending(g => g.PurchaseDate),
                _ => games.OrderByDescending(g => g.PurchaseDate)
            };

            // Apply pagination
            var totalItems = games.Count();
            var pagedGames = games
                .Skip((pagedRequestDto.PageNumber - 1) * pagedRequestDto.PageSize)
                .Take(pagedRequestDto.PageSize)
                .ToList();

            var result = new PagedResult<LibraryGameResponseDto>
            {
                Items = pagedGames.Select(MapToResponse),
                PageNumber = pagedRequestDto.PageNumber,
                PageSize = pagedRequestDto.PageSize,
                TotalItems = totalItems
            };

            return OperationResult<PagedResult<LibraryGameResponseDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar biblioteca do jogador: {PlayerId}", playerId);
            return OperationResult<PagedResult<LibraryGameResponseDto>>.Failure("Erro ao buscar biblioteca do jogador.");
        }
    }

    public async Task<OperationResult<bool>> OwnsGameAsync(Guid playerId, Guid gameId)
    {
        try
        {
            var ownsGame = await _libraryRepository.OwnsGameAsync(playerId, gameId);
            return OperationResult<bool>.Success(ownsGame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar propriedade do jogo: {GameId} para o jogador: {PlayerId}", gameId, playerId);
            return OperationResult<bool>.Failure("Erro ao verificar propriedade do jogo.");
        }
    }

    public async Task<OperationResult<bool>> CanPurchaseGameAsync(Guid playerId, Guid gameId)
    {
        try
        {
            var galleryGame = await _galleryRepository.GetGalleryGameByIdAsync(gameId);
            if (galleryGame == null)
                return OperationResult<bool>.Success(false);

            var ownsGame = await _libraryRepository.HasGameInLibraryAsync(playerId, galleryGame.Name);
            var isAvailable = await _galleryRepository.IsAvailableForPurchaseAsync(gameId);

            return OperationResult<bool>.Success(!ownsGame && isAvailable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar disponibilidade para compra: {GameId} para o jogador: {PlayerId}", gameId, playerId);
            return OperationResult<bool>.Failure("Erro ao verificar disponibilidade para compra.");
        }
    }

    public async Task<OperationResult<int>> GetLibraryGameCountAsync(Guid playerId)
    {
        try
        {
            var count = await _libraryRepository.GetLibraryGameCountAsync(playerId);
            return OperationResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao contar jogos na biblioteca do jogador: {PlayerId}", playerId);
            return OperationResult<int>.Failure("Erro ao contar jogos na biblioteca.");
        }
    }

    public async Task<OperationResult<decimal>> GetTotalSpentAsync(Guid playerId)
    {
        try
        {
            var total = await _libraryRepository.GetTotalSpentAsync(playerId);
            return OperationResult<decimal>.Success(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular total gasto pelo jogador: {PlayerId}", playerId);
            return OperationResult<decimal>.Failure("Erro ao calcular total gasto.");
        }
    }

    public async Task<OperationResult<IEnumerable<LibraryGameResponseDto>>> GetRecentPurchasesAsync(Guid playerId, int count = 5)
    {
        try
        {
            var games = await _libraryRepository.GetRecentPurchasesAsync(playerId, count);
            var response = games.Select(MapToResponse);
            return OperationResult<IEnumerable<LibraryGameResponseDto>>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar compras recentes do jogador: {PlayerId}", playerId);
            return OperationResult<IEnumerable<LibraryGameResponseDto>>.Failure("Erro ao buscar compras recentes.");
        }
    }

    private LibraryGameResponseDto MapToResponse(LibraryGame game)
    {
        return new LibraryGameResponseDto
        {
            Id = game.Id,
            EAN = game.EAN,
            Name = game.Name,
            Genre = game.Genre,
            Description = game.Description,
            PlayerId = game.PlayerId,
            PlayerDisplayName = game.Player?.DisplayName ?? string.Empty,
            PurchaseDate = game.PurchaseDate,
            PurchasePrice = game.PurchasePrice
        };
    }
}