using FCG.Application.Dto.Request;
using FCG.Application.Dto.Response;

public static class GameMapper
{
    public static void MapFromRequest(this GameResponseDto target, GameUpdateRequestDto source)
    {
        if (!string.IsNullOrWhiteSpace(source.EAN))
            target.EAN = source.EAN;

        if (!string.IsNullOrWhiteSpace(source.Title))
            target.Title = source.Title;

        if (!string.IsNullOrWhiteSpace(source.SubTitle))
            target.SubTitle = source.SubTitle;

        if (!string.IsNullOrWhiteSpace(source.Genre))
            target.Genre = source.Genre;

        if (!string.IsNullOrWhiteSpace(source.Description))
            target.Description = source.Description;
    }
}
