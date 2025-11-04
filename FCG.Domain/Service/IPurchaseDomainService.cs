using FCG.Domain.Entities;

namespace FCG.Domain.Service;

public interface IPurchaseDomainService
{
    Purchase RegisterPurchase(Player player, GalleryGame game);
}
