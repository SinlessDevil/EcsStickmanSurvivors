using Code.Gameplay.Features.Enemies.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Enemies
{
    public class EnemyFeature : Feature
    {
        public EnemyFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<InitializeEnemySpawnTimerSystem>());
            Add(systemFactory.Create<InitializeEnemyBufferTimerSystem>());
            
            Add(systemFactory.Create<EnemySpawnSystem>());
            
            Add(systemFactory.Create<EnemyChaseHeroSystem>());
            Add(systemFactory.Create<EnemyDeathSystem>());
            Add(systemFactory.Create<EnemyDropLootSystem>());

            Add(systemFactory.Create<EnemyShamanBehaviorSystem>());

            Add(systemFactory.Create<AnimateEnemyMovementSystem>());
            
            Add(systemFactory.Create<FinalizeEnemyDeathProcessingSystem>());
        }
    }
}