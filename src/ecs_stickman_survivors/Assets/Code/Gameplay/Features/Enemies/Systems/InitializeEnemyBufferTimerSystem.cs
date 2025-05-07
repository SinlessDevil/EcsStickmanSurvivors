using Code.Common.Entity;
using Entitas;

namespace Code.Gameplay.Features.Enemies.Systems
{
    public class InitializeEnemyBufferTimerSystem : IInitializeSystem
    {
        private const float EnemyBufferTimer = 3;
        
        public void Initialize()
        {
            CreateEntity.Empty().AddBuffTimer(EnemyBufferTimer);
        }
    }
}