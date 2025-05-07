using Code.Common.Entity;
using Entitas;

namespace Code.Gameplay.Features.Boosters.Systems
{
    public class InitializeBoosterSpawnTimerSystem : IInitializeSystem
    {
        private const float BoosterSpawnTimer = 5;
        
        public void Initialize()
        {
            CreateEntity.Empty().AddBoosterSpawnTimer(BoosterSpawnTimer);
        }
    }
}