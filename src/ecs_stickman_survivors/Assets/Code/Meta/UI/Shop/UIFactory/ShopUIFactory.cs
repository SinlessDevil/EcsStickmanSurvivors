using Code.Gameplay.StaticData;
using Code.Infrastructure.AssetManagement;
using Code.Meta.UI.Shop.Items;
using UnityEngine;
using Zenject;

namespace Code.Meta.UI.Shop.UIFactory
{
    public class ShopUIFactory : IShopUIFactory
    {
        private const string ShopItemPath = "UI/Home/Shop/ShopItem";
        
        private readonly IInstantiator _instantiator;
        private readonly IAssetProvider _assetProvider;
        private readonly IStaticDataService _staticDataService;

        public ShopUIFactory(
            IInstantiator instantiator, 
            IAssetProvider assetProvider,
            IStaticDataService staticDataService)
        {
            _instantiator = instantiator;
            _assetProvider = assetProvider;
            _staticDataService = staticDataService;
        }

        public ShopItem CreateShopItem(ShopItemConfig config, Transform parent)
        {
            ShopItem shopItem = _assetProvider.LoadAsset<ShopItem>(ShopItemPath);
            ShopItem item = _instantiator.InstantiatePrefabForComponent<ShopItem>(shopItem, parent);

            item.Setup(config);
            
            return item;
        }
    }
}