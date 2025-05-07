using Code.Common.Entity;
using Code.Meta.UI.Shop.Items;
using Code.Meta.UI.Shop.Service;
using Entitas;

namespace Code.Meta.UI.Shop.Systems
{
    public class BuyItemOnRequestSystem : IExecuteSystem
    {
        private readonly IShopUIService _shopUIService;
        private readonly IGroup<MetaEntity> _shopItemPurchaseRequests;
        private readonly IGroup<MetaEntity> _storages;

        public BuyItemOnRequestSystem(MetaContext game, IShopUIService shopUIService)
        {
            _shopUIService = shopUIService;
            
            _storages = game.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Storage, MetaMatcher.Gold));
            
            _shopItemPurchaseRequests = game.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.BuyRequest, MetaMatcher.ShopItemId));
        }

        public void Execute()
        {
            foreach (MetaEntity storage in _storages)
            foreach (MetaEntity request in _shopItemPurchaseRequests)
            {
                ShopItemConfig config = _shopUIService.GetConfig(request.ShopItemId);
                
                if(storage.Gold >= config.Price)
                {
                    storage.ReplaceGold(storage.Gold - config.Price);
                    
                    CreateMetaEntity.Empty()
                        .AddShopItemId(request.ShopItemId)
                        .isPurchased = true;
                    
                    _shopUIService.UpdatePurchasedItem(request.ShopItemId);
                }
                
                request.isDestructed = true;
            }
        }
    }
}