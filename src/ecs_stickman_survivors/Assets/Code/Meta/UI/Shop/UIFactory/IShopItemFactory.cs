using Code.Meta.UI.Shop.Items;

namespace Code.Meta.UI.Shop.UIFactory
{
    public interface IShopItemFactory
    {
        MetaEntity CreateShopItem(ShopItemId shopItemId);
    }
}