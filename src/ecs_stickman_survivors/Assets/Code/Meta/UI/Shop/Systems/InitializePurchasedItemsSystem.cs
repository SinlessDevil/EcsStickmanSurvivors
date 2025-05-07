using System.Linq;
using Code.Meta.UI.Shop.Service;
using Entitas;

namespace Code.Meta.UI.Shop.Systems
{
    public class InitializePurchasedItemsSystem : IInitializeSystem
    {
        private readonly IShopUIService _shopUIService;
        
        private readonly IGroup<MetaEntity> _purchasedItems;

        public InitializePurchasedItemsSystem(MetaContext meta, IShopUIService shopUIService)
        {
            _shopUIService = shopUIService;
            _purchasedItems = meta.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.ShopItemId, 
                    MetaMatcher.Purchased));
        }

        public void Initialize()
        {
            _shopUIService.UpdatePurchasedItems(_purchasedItems.GetEntities().Select(x => x.ShopItemId));
        }
    }
}