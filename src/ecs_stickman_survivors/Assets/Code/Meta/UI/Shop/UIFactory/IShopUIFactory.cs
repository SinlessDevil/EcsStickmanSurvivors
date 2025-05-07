using Code.Meta.UI.Shop.Items;
using UnityEngine;

namespace Code.Meta.UI.Shop.UIFactory
{
    public interface IShopUIFactory
    {
        ShopItem CreateShopItem(ShopItemConfig config, Transform parent);
    }
}