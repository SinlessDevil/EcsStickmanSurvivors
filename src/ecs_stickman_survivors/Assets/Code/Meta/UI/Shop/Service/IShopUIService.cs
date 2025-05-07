using System;
using System.Collections.Generic;
using Code.Meta.UI.Shop.Items;

namespace Code.Meta.UI.Shop.Service
{
    public interface IShopUIService
    {
        event Action ShopChangedEvent;
        List<ShopItemConfig> GetAvailableShopItems { get; }
        
        ShopItemConfig GetConfig(ShopItemId requestShopItemId);
        void UpdatePurchasedItems(IEnumerable<ShopItemId> purchasedItems);
        void UpdatePurchasedItem(ShopItemId requestShopItemId);
        
        void Cleanup();
    }
}