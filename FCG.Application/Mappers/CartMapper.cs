using FCG.Application.Dto.Response;
using FCG.Domain.Entities;

namespace FCG.Application.Mappers;

public static class CartMapper
{
    public static CartResponseDto ToDto(Cart cart)
    {
        if (cart == null)
            return null!;

        return new CartResponseDto
        {
            Id = cart.Id,
            PlayerId = cart.PlayerId,
            Items = cart.Items.Select(ToDto).ToList()
        };
    }

    private static CartItemResponseDto ToDto(CartItem item)
    {
        return new CartItemResponseDto
        {
            GameId = item.Game.Id,
            Name = item.Game.Title,
            Genre = item.Game.Genre,
            Description = item.Game.Description ?? string.Empty,
            Price = 0 // O preço deve vir da GalleryGame, não do Game
        };
    }
}