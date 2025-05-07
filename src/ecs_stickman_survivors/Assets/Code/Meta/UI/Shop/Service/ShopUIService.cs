using System;
using System.Collections.Generic;
using Code.Gameplay.StaticData;
using Code.Meta.UI.Shop.Items;

namespace Code.Meta.UI.Shop.Service
{
    public class ShopUIService : IShopUIService
    {
        private readonly List<ShopItemId> _purchasedItems = new();
        private readonly Dictionary<ShopItemId, ShopItemConfig> _availableItems = new();
    
        private readonly IStaticDataService _staticData;
        
    
        public ShopUIService(IStaticDataService staticData) => 
            _staticData = staticData;

        public void UpdatePurchasedItems(IEnumerable<ShopItemId> purchasedItems)
        {
            _purchasedItems.AddRange(purchasedItems);

            RefreshAvailableItems();
        }

        public void UpdatePurchasedItem(ShopItemId shopItemId)
        {
            _availableItems.Remove(shopItemId);
            _purchasedItems.Add(shopItemId);
      
            ShopChangedEvent?.Invoke();
        }

        public event Action ShopChangedEvent;

        public List<ShopItemConfig> GetAvailableShopItems => 
            new(_availableItems.Values);

        public ShopItemConfig GetConfig(ShopItemId shopItemId) => 
            _availableItems.GetValueOrDefault(shopItemId);

        public void Cleanup()
        {
            _purchasedItems.Clear();
            _availableItems.Clear();
      
            ShopChangedEvent = null;
        }

        private void RefreshAvailableItems()
        {
            foreach (ShopItemConfig itemConfig in _staticData.GetShopItemConfigs())
            {
                if(!_purchasedItems.Contains(itemConfig.ShopItemId))
                    _availableItems.Add(itemConfig.ShopItemId, itemConfig);
            }
      
            ShopChangedEvent?.Invoke();
        }
    }
}