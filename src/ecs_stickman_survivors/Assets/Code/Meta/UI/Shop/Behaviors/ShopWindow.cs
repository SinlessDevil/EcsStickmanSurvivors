using System;
using System.Collections.Generic;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Services;
using Code.Meta.UI.GoldHolder.Service;
using Code.Meta.UI.Shop.Items;
using Code.Meta.UI.Shop.Service;
using Code.Meta.UI.Shop.UIFactory;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Behaviors
{
    public class ShopWindow : BaseWindow
    {
        public ResourceTypeId ResourceTypeId;
        public Transform ItemsLayout;
        public Button CloseButton;
        public GameObject NoItemsAvailable;

        private List<ShopItem> _items = new();

        private IWindowService _windowService;
        private IShopUIFactory _shopUIFactory;
        private IShopUIService _shopUIService;
        private IStorageUIService _storageUIService;

        [Inject]
        private void Construct(
            IWindowService windowService,
            IShopUIFactory shopUIFactory,
            IShopUIService shopUIService,
            IStorageUIService storageUIService)
        {
            Id = WindowId.LevelUpWindow;

            _windowService = windowService;
            _shopUIFactory = shopUIFactory;
            _shopUIService = shopUIService;
            _storageUIService = storageUIService;
        }

        protected override void Initialize()
        {
            CloseButton.onClick.AddListener(Close);
        }

        protected override void SubscribeUpdates()
        {
            _shopUIService.ShopChangedEvent += OnRefresh;
            _storageUIService.ResourceBoostChangedEvent += OnUpdateBoostState;

            OnRefresh();
        }

        protected override void UnsubscribeUpdates()
        {
            _shopUIService.ShopChangedEvent -= OnRefresh;
            _storageUIService.ResourceBoostChangedEvent -= OnUpdateBoostState;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            CloseButton.onClick.RemoveListener(Close);
        }

        private void OnUpdateBoostState(ResourceTypeId resourceTypeId)
        {
            if(resourceTypeId != ResourceTypeId)
                return;
            
            bool itemsCanBeBought = Math.Abs(_storageUIService.GetResourceBoost(ResourceTypeId) - 0) <= float.Epsilon;

            foreach (ShopItem shopItem in _items)
            {
                shopItem.OnUpdateAvailability(itemsCanBeBought);
            }
        }

        private void OnRefresh()
        {
            ClearItems();

            List<ShopItemConfig> availableItems = _shopUIService.GetAvailableShopItems;

            NoItemsAvailable.SetActive(availableItems.Count == 0);

            FillItems(availableItems);

            OnUpdateBoostState(ResourceTypeId);
        }

        private void ClearItems()
        {
            _items.ForEach(x => Destroy(x.gameObject));
            _items.Clear();
        }

        private void FillItems(List<ShopItemConfig> availableItems)
        {
            foreach (ShopItemConfig availableItem in availableItems)
                _items.Add(_shopUIFactory.CreateShopItem(availableItem, ItemsLayout));
        }

        private void Close()
        {
            _windowService.Close(Id);
        }
    }
}