using Code.Infrastructure.Systems;
using Code.Meta.UI.Shop.Systems;

namespace Code.Meta.UI.Shop
{
    public class ShopFeature : Feature
    {
        public ShopFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<BuyItemOnRequestSystem>());
            Add(systemFactory.Create<ProcessBoughtItemsSystem>());
        }
    }
}