using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.StaticData;
using Entitas;

namespace Code.Gameplay.Features.Enemies.Systems
{
    public class EnemyDropLootSystem : IExecuteSystem
    {
        private readonly ILootFactory _lootFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly IGroup<GameEntity> _enemies;
        private readonly List<GameEntity> _buffer = new(128);

        public EnemyDropLootSystem(GameContext game, 
            ILootFactory lootFactory,
            IStaticDataService staticDataService)
        {
            _lootFactory = lootFactory;
            _staticDataService = staticDataService;

            _enemies = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Enemy,
                    GameMatcher.Dead,
                    GameMatcher.ProcessingDeath,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity enemy in _enemies.GetEntities(_buffer))
            {
                LootTypeId selectedLoot = GetRandomLoot();
                _lootFactory.CreateLootItem(selectedLoot, enemy.WorldPosition);
            }
        }

        private LootTypeId GetRandomLoot()
        {
            float totalWeight = 0f;
            foreach (var loot in _staticDataService.EnemyDropConfig.LootTable)
                totalWeight += loot.DropChance;

            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            float cumulativeWeight = 0f;

            foreach (var loot in _staticDataService.EnemyDropConfig.LootTable)
            {
                cumulativeWeight += loot.DropChance;
                if (randomValue < cumulativeWeight)
                    return loot.LootType;
            }

            return LootTypeId.ExpGem;
        }
    }
}