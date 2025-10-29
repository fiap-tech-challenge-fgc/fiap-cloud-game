using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Repository;
using FCG.Application.Interfaces.Service;
using FCG.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FCG.Application.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository playerRepository,
        ILogger<PlayerService> logger)
    {
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<OperationResult<PlayerWithUserResponseDto>> GetByIdAsync(Guid playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            _logger.LogWarning("Jogador não encontrado: {PlayerId}", playerId);
            return OperationResult<PlayerWithUserResponseDto>.Failure("Jogador não encontrado.");
        }

        var dto = new PlayerWithUserResponseDto
        {
            PlayerId = player.Id,
            UserId = player.UserId,
            DisplayName = player.DisplayName,
            GamesCount = player.Library?.Count ?? 0
        };

        return OperationResult<PlayerWithUserResponseDto>.Success(dto);
    }

    public async Task<OperationResult> CreateAsync(PlayerCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DisplayName))
            return OperationResult.Failure("DisplayName é obrigatório.");

        if (await _playerRepository.ExistsByUserIdAsync(dto.UserId))
        {
            _logger.LogWarning("Tentativa de criar jogador duplicado para usuário: {UserId}", dto.UserId);
            return OperationResult.Failure("Já existe um jogador para este usuário.");
        }

        try
        {
            var player = new Player(dto.UserId, dto.DisplayName);
            await _playerRepository.AddAsync(player);

            _logger.LogInformation("Jogador criado com sucesso: {PlayerId} para usuário: {UserId}", player.Id, dto.UserId);
            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar jogador para usuário: {UserId}", dto.UserId);
            return OperationResult.Failure("Erro ao criar jogador.");
        }
    }

    public async Task<OperationResult> UpdateDisplayNameAsync(Guid playerId, string newDisplayName)
    {
        if (string.IsNullOrWhiteSpace(newDisplayName))
            return OperationResult.Failure("DisplayName não pode ser vazio.");

        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            _logger.LogWarning("Tentativa de atualizar DisplayName de jogador inexistente: {PlayerId}", playerId);
            return OperationResult.Failure("Jogador não encontrado.");
        }

        try
        {
            player.DisplayNameChange(newDisplayName);
            await _playerRepository.UpdateAsync(player);

            _logger.LogInformation("DisplayName atualizado com sucesso para jogador: {PlayerId}", playerId);
            return OperationResult.Success();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "DisplayName inválido para jogador: {PlayerId}", playerId);
            return OperationResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar DisplayName do jogador: {PlayerId}", playerId);
            return OperationResult.Failure("Erro ao atualizar DisplayName.");
        }
    }

    public async Task<OperationResult<IEnumerable<PlayerResponseDto>>> GetAllAsync()
    {
        try
        {
            var players = await _playerRepository.GetAllAsync();
            var dtoList = players.Select(p => new PlayerResponseDto
            {
                Id = p.Id,
                DisplayName = p.DisplayName,
                GamesCount = p.Library?.Count ?? 0
            });

            return OperationResult<IEnumerable<PlayerResponseDto>>.Success(dtoList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar lista de jogadores");
            return OperationResult<IEnumerable<PlayerResponseDto>>.Failure("Erro ao buscar jogadores.");
        }
    }

    public async Task<bool> ExistsAsync(Guid playerId)
    {
        return await _playerRepository.ExistsAsync(playerId);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId)
    {
        return await _playerRepository.ExistsByUserIdAsync(userId);
    }
}