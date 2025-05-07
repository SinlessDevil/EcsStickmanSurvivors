using Code.Infrastructure.Systems;
using Code.Meta.UI.ResourceHolder.Systems;
using Code.Meta.UI.Shop;
using Code.Meta.UI.Shop.Systems;

namespace Code.Meta
{
    public class HomeUIFeature : Feature
    {
        public HomeUIFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<InitializePurchasedItemsSystem>());
            
            Add(systemFactory.Create<RefreshGoldGainBoostSystem>());
            Add(systemFactory.Create<RefreshGemGainBoostSystem>());
            
            Add(systemFactory.Create<RefreshGoldSystem>());
            Add(systemFactory.Create<RefreshGemSystem>());
            
            Add(systemFactory.Create<ShopFeature>());
        }
    }
}