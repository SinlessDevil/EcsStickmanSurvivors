using Code.Common.Entity;
using Entitas;

namespace Code.Gameplay.Features.Enemies.Systems
{
    public class InitializeEnemySpawnTimerSystem : IInitializeSystem
    {
        private const float EnemySpawnTimer = 1;
        
        public void Initialize()
        {
            CreateEntity.Empty().AddSpawnTimer(EnemySpawnTimer);
        }
    }
}