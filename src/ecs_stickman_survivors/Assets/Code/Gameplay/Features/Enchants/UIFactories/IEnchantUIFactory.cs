using Code.Gameplay.Features.Enchants.Behaviors;
using UnityEngine;

namespace Code.Gameplay.Features.Enchants.UIFactories
{
    public interface IEnchantUIFactory
    {
        Enchant CreateEnchant(Transform transform, EnchantTypeId enchantTypeId);
    }
}