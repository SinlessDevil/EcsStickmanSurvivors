using Code.Common.Extensions;
using Code.Gameplay.Cameras.Provider;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Enemies.Factory;
using Code.Gameplay.Features.Enemies.Configs;
using Code.Gameplay.Features.LevelUp.Services;
using Code.Gameplay.StaticData;
using Entitas;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Features.Enemies.Systems
{
    public class EnemySpawnSystem : IExecuteSystem
    {
        private const float SpawnDistanceGap = 0.5f;
        
        private readonly ITimeService _timeService;
        private readonly IEnemyFactory _enemyFactory;
        private readonly ICameraProvider _cameraProvider;
        private readonly ILevelUpService _levelUpService;
        private readonly IStaticDataService _staticDataService;

        private readonly IGroup<GameEntity> _timers;
        private readonly IGroup<GameEntity> _heroes;
        
        public EnemySpawnSystem(GameContext game, 
            ITimeService timeService, 
            IEnemyFactory enemyFactory,
            ICameraProvider cameraProvider,
            ILevelUpService levelUpService,
            IStaticDataService staticDataService)
        {
            _timeService = timeService;
            _enemyFactory = enemyFactory;
            _cameraProvider = cameraProvider;
            _levelUpService = levelUpService;
            _staticDataService = staticDataService;

            _timers = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.SpawnTimer));
            
            _heroes = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Hero,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity hero in _heroes)
            foreach (GameEntity timer in _timers)
            {
                timer.ReplaceSpawnTimer(timer.SpawnTimer - _timeService.DeltaTime);
        
                if (timer.SpawnTimer <= 0)
                {
                    int currentLevel = _levelUpService.CurrentLevel;
                    EnemyWave wave = _staticDataService.GetCurrentWave(currentLevel);
                    timer.ReplaceSpawnTimer(wave.SpawnInterval);
                    SpawnEnemies(currentLevel, wave,hero.WorldPosition);
                }
            }
        }

        private void SpawnEnemies(int currentLevel, EnemyWave wave, Vector2 heroPosition)
        {
            if (wave == null) return;

            Vector2 spawnPosition = GetSpawnPosition(heroPosition);
            foreach (var enemyData in wave.Enemies)
            {
                int enemyCount = Random.Range(enemyData.MinCount, enemyData.MaxCount + 1);
                for (int i = 0; i < enemyCount; i++)
                {
                    Vector2 offset = GetSpawnOffset();
                    _enemyFactory.CreateEnemy(enemyData.EnemyType, spawnPosition + offset, currentLevel);
                }
            }
        }

        private Vector2 GetSpawnPosition(Vector2 heroPosition)
        {
            bool startWithHorizontal = Random.Range(0, 2) == 0;
            return startWithHorizontal
                ? HorizontalSpawnPosition(heroPosition)
                : VerticalSpawnPosition(heroPosition);
        }

        private Vector2 HorizontalSpawnPosition(Vector2 aroundPosition)
        {
            Vector2[] horizontalDirections = { Vector2.left, Vector2.right };
            Vector2 primaryDirection = horizontalDirections.PickRandom();
            float horizontalOffsetDistance = _cameraProvider.WorldScreenWidth / 2 + SpawnDistanceGap;
            float verticalRandomOffset = Random.Range(-_cameraProvider.WorldScreenHeight / 2, _cameraProvider.WorldScreenHeight / 2);
            return aroundPosition + primaryDirection * horizontalOffsetDistance + Vector2.up * verticalRandomOffset;
        }

        private Vector2 VerticalSpawnPosition(Vector2 aroundPosition)
        {
            Vector2[] verticalDirections = { Vector2.up, Vector2.down };
            Vector2 primaryDirection = verticalDirections.PickRandom();
            float verticalOffsetDistance = _cameraProvider.WorldScreenHeight / 2 + SpawnDistanceGap;
            float horizontalRandomOffset = Random.Range(-_cameraProvider.WorldScreenWidth / 2, _cameraProvider.WorldScreenWidth / 2);
            return aroundPosition + primaryDirection * verticalOffsetDistance + Vector2.right * horizontalRandomOffset;
        }

        private Vector2 GetSpawnOffset()
        {
            return Random.insideUnitCircle * Random.Range(0.5f, 2f);
        }
    }
}
