using UnityEngine;

namespace Code.Meta.UI.Shop.Items
{
  [CreateAssetMenu(fileName = "shopItemConfig", menuName = "ECS Survivors/ShopItem Config")]
  public class ShopItemConfig : ScriptableObject
  {
    public ShopItemId ShopItemId;
    public ShopItemKind Kind;

    public Sprite Icon;
    public int Price;

    public float Duration;
    public float Boost;
  }
}