using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Configs
{
    [CreateAssetMenu(menuName = "ECS Survivors/EnemyDrop", fileName = "EnemyDropConfig")]
    public class EnemyDropConfig : ScriptableObject
    {
        public List<LootDropEntry> LootTable = new()
        {
            new LootDropEntry { LootType = LootTypeId.HealingItem, DropChance = 25f },
            new LootDropEntry { LootType = LootTypeId.PoisonEnchantItem, DropChance = 15f },
            new LootDropEntry { LootType = LootTypeId.ExplosionEnchantItem, DropChance = 15f },
            new LootDropEntry { LootType = LootTypeId.HexEnchantItem, DropChance = 25f },
            new LootDropEntry { LootType = LootTypeId.ExpGem, DropChance = 75f },
        };
    }

    [Serializable]
    public class LootDropEntry
    {
        public LootTypeId LootType;
        public float DropChance;
    }
}