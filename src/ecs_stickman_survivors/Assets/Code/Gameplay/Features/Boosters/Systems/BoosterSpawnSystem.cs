using Code.Gameplay.Cameras.Provider;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Factory;
using Entitas;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Features.Boosters.Systems
{
    public class BoosterSpawnSystem : IExecuteSystem
    {
        private const float SpawnDistanceGap = 0.5f;
        private const float BoosterSpawnTimer = 10;
        
        private readonly ITimeService _timeService;
        private readonly ILootFactory _lootFactory;
        private readonly ICameraProvider _cameraProvider;
        
        private readonly IGroup<GameEntity> _timers;
        private readonly IGroup<GameEntity> _heroes;
        
        public BoosterSpawnSystem(GameContext game, 
            ITimeService timeService, 
            ILootFactory lootFactory,
            ICameraProvider cameraProvider)
        {
            _timeService = timeService;
            _lootFactory = lootFactory;
            _cameraProvider = cameraProvider;
            
            _timers = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.BoosterSpawnTimer));
            
            _heroes = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Hero,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity hero in _heroes)
            foreach (GameEntity timer in _timers)
            {
                timer.ReplaceBoosterSpawnTimer(timer.BoosterSpawnTimer - _timeService.DeltaTime);
                if (timer.BoosterSpawnTimer <= 0)
                {
                    timer.ReplaceBoosterSpawnTimer(BoosterSpawnTimer);
                    
                    _lootFactory.CreateLootItem(LootTypeId.MaxHPUpItem, at: RandomSpawnPosition(hero.WorldPosition));
                    _lootFactory.CreateLootItem(LootTypeId.InvulnerabilityItem, at: RandomSpawnPosition(hero.WorldPosition));
                }
            }
        }
        
        private Vector2 RandomSpawnPosition(Vector2 aroundPosition)
        {
            float xMin = aroundPosition.x - _cameraProvider.WorldScreenWidth / 2 + SpawnDistanceGap;
            float xMax = aroundPosition.x + _cameraProvider.WorldScreenWidth / 2 - SpawnDistanceGap;
            float yMin = aroundPosition.y - _cameraProvider.WorldScreenHeight / 2 + SpawnDistanceGap;
            float yMax = aroundPosition.y + _cameraProvider.WorldScreenHeight / 2 - SpawnDistanceGap;

            float randomX = Random.Range(xMin, xMax);
            float randomY = Random.Range(yMin, yMax);

            return new Vector2(randomX, randomY);
        }
    }
}
