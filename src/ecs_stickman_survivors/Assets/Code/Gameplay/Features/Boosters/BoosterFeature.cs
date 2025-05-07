using Code.Gameplay.Features.Boosters.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Boosters
{
    public class BoosterFeature : Feature
    {
        public BoosterFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<InitializeBoosterSpawnTimerSystem>());
            Add(systemFactory.Create<BoosterSpawnSystem>());
        }
    }
}