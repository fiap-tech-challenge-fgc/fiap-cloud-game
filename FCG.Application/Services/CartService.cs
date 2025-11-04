using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;
using FCG.Application.Dto.Result;
using FCG.Application.Interfaces.Repository;
using FCG.Application.Interfaces.Service;
using FCG.Application.Mappers;
using FCG.Domain.Service;

namespace FCG.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IGalleryRepository _galleryRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICartDomainService _cartDomainService;

    public CartService(
        ICartRepository cartRepository,
        IGameRepository gameRepository,
        IGalleryRepository galleryRepository,
        IPlayerRepository playerRepository,
        ICartDomainService cartDomainService)
    {
        _cartRepository = cartRepository;
        _gameRepository = gameRepository;
        _galleryRepository = galleryRepository;
        _playerRepository = playerRepository;
        _cartDomainService = cartDomainService;
    }

    public async Task<OperationResult> AddItemAsync(CartItemRequestDto request)
    {
        // Verifica se o player existe
        var user = await _playerRepository.GetByUserIdAsync(request.UserId);
        if (user == null)
            return OperationResult.Failure("User não encontrado. Por favor, complete o cadastro do perfil.");

        var game = await _gameRepository.GetByIdAsync(request.GameId);
        if (game == null)
            return OperationResult.Failure("Jogo não encontrado.");

        var cart = await _cartRepository.GetByPlayerIdAsync(user.Id);
        if (cart == null)
        {
            cart = _cartDomainService.CreateCart(user.Id);
            _cartDomainService.AddItemToCart(cart, game);
            await _cartRepository.AddAsync(cart);
        }
        else
        {
            _cartDomainService.AddItemToCart(cart, game);
            await _cartRepository.UpdateAsync(cart);
        }

        return OperationResult.Success();
    }

    public async Task<OperationResult> RemoveItemAsync(CartItemRequestDto request)
    {
        var cart = await _cartRepository.GetByPlayerIdAsync(request.PlayerId);
        if (cart == null)
            return OperationResult.Failure("Carrinho não encontrado.");

        _cartDomainService.RemoveItemFromCart(cart, request.GameId);
        await _cartRepository.UpdateAsync(cart);

        return OperationResult.Success();
    }

    public async Task<OperationResult<CartResponseDto>> GetCartByPlayerIdAsync(Guid playerId)
    {
        var cart = await _cartRepository.GetByPlayerIdAsync(playerId);
        if (cart == null)
            return OperationResult<CartResponseDto>.Success(new CartResponseDto
            {
                PlayerId = playerId,
                Items = []
            });

        var cartDto = CartMapper.ToDto(cart);

        // Busca os preços da galeria para cada item do carrinho
        foreach (var item in cartDto.Items)
        {
            var galleryGame = await _galleryRepository.GetByGameIdAsync(item.GameId);
            if (galleryGame != null)
            {
                item.Price = galleryGame.FinalPrice;
            }
        }

        return OperationResult<CartResponseDto>.Success(cartDto);
    }

    public async Task<OperationResult<CartResponseDto>> GetCartByUserIdAsync(Guid userId)
    {
        var player = await _playerRepository.GetByUserIdAsync(userId);
        if (player == null)
            return OperationResult<CartResponseDto>.Failure("Player não encontrado. Por favor, complete o cadastro do perfil.");

        var cart = await _cartRepository.GetByPlayerIdAsync(player.Id);
        if (cart == null)
            return OperationResult<CartResponseDto>.Success(new CartResponseDto
            {
                PlayerId = player.Id,
                Items = []
            });

        var cartDto = CartMapper.ToDto(cart);

        // Busca os preços da galeria para cada item do carrinho
        foreach (var item in cartDto.Items)
        {
            var galleryGame = await _galleryRepository.GetByGameIdAsync(item.GameId);
            if (galleryGame != null)
            {
                item.Price = galleryGame.FinalPrice;
            }
        }

        return OperationResult<CartResponseDto>.Success(cartDto);
    }

    public async Task<OperationResult> ClearCartAsync(Guid playerId)
    {
        var cart = await _cartRepository.GetByPlayerIdAsync(playerId);
        if (cart == null)
            return OperationResult.Failure("Carrinho não encontrado.");

        _cartDomainService.ClearCart(cart);
        await _cartRepository.UpdateAsync(cart);

        return OperationResult.Success();
    }
}
