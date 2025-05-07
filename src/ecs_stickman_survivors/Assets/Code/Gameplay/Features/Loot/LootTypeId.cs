using System;

namespace Code.Gameplay.Features.Loot
{
    [Serializable]
    public enum LootTypeId
    {
        Unknown = 0,
        ExpGem = 1,
        HealingItem = 2,
        PoisonEnchantItem = 3,
        ExplosionEnchantItem = 4,
        MaxHPUpItem = 5,
        InvulnerabilityItem = 6,
        HexEnchantItem = 7,
    }
}