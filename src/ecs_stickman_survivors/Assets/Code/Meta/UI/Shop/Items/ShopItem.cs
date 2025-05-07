using System;
using Code.Common.Entity;
using Code.Meta.UI.GoldHolder.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

namespace Code.Meta.UI.Shop.Items
{
    public class ShopItem : MonoBehaviour
    {
        public ResourceTypeId ResourceTypeId;
        [HideInInspector] public ShopItemId ItemId;
        [Space(10)]
        public Image Icon;
        public TextMeshProUGUI PriceText;
        public TextMeshProUGUI DurationText;
        public TextMeshProUGUI BoostText;
        public Button BuyButton;
        public CanvasGroup CanvasBuyGroup;
        [Space(10)]
        public Color EnoughGoldColor = Color.white;
        public Color NotEnoughGoldColor = Color.red;
        
        private bool _isAvailable;
        private int _price;
        private float _currentGold;
        
        private IStorageUIService _storageUIService;

        [Inject]
        private void Construct(IStorageUIService storageUIService)
        {
            _storageUIService = storageUIService;
        }
        
        public void Setup(ShopItemConfig config)
        {
            ItemId = config.ShopItemId;
            
            Icon.sprite = config.Icon;
            PriceText.text = config.Price.ToString();
            DurationText.text = TimeSpan.FromSeconds(config.Duration).ToString("m'm 's's'");
            BoostText.text = config.Boost.ToString("+0%");
            
            _price = config.Price;
            
            BuyButton.onClick.AddListener(OnBuyItem);
        }
        
        private void Start()
        {
            _storageUIService.ResourceChangedEvent += OnUpdatePriceThreshold;
            
            OnUpdatePriceThreshold(ResourceTypeId);
        }

        private void OnDestroy()
        {
            _storageUIService.ResourceChangedEvent -= OnUpdatePriceThreshold;
            BuyButton.onClick.RemoveListener(OnBuyItem);
        }
        
        public void OnUpdateAvailability(bool itemsCanBeBought)
        {
            _isAvailable = itemsCanBeBought;
            CanvasBuyGroup.alpha = _isAvailable ? 1f : 0.7f;
            
            RefreshBuyButton();
        }

        private void OnUpdatePriceThreshold(ResourceTypeId resourceTypeId)
        {
            if(ResourceTypeId != resourceTypeId)
                return;
            
            _currentGold = _storageUIService.GetResource(resourceTypeId);
            
            PriceText.color = EnoughGold ? EnoughGoldColor : NotEnoughGoldColor;
            
            RefreshBuyButton();
        }

        private bool EnoughGold => _currentGold >= _price;

        private void RefreshBuyButton() => BuyButton.interactable = EnoughGold & _isAvailable;
        
        private void OnBuyItem()
        {
            CreateMetaEntity.Empty()
                .AddShopItemId(ItemId)
                .isBuyRequest = true;
        }
    }
}