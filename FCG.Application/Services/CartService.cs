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
    private readonly ICartDomainService _cartDomainService;

    public CartService(
        ICartRepository cartRepository,
        IGameRepository gameRepository,
        ICartDomainService cartDomainService)
    {
        _cartRepository = cartRepository;
        _gameRepository = gameRepository;
        _cartDomainService = cartDomainService;
    }

    public async Task<OperationResult> AddItemAsync(CartItemRequestDto request)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);
        if (game == null)
            return OperationResult.Failure("Jogo não encontrado.");

        var cart = await _cartRepository.GetByPlayerIdAsync(request.PlayerId);
        if (cart == null)
        {
            cart = _cartDomainService.CreateCart(request.PlayerId);
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

    public async Task<OperationResult<CartResponseDto>> GetCartAsync(Guid playerId)
    {
        var cart = await _cartRepository.GetByPlayerIdAsync(playerId);
        if (cart == null)
            return OperationResult<CartResponseDto>.Success(new CartResponseDto 
            { 
                PlayerId = playerId,
                Items = []
            });

        return OperationResult<CartResponseDto>.Success(CartMapper.ToDto(cart));
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
