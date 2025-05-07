using Code.Gameplay.Features.LifeTime.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.LifeTime
{
    public sealed class DeathFeature  : Feature
    {
        public DeathFeature(ISystemFactory systems)
        {
            Add(systems.Create<MarkDeadSystem>());
            Add(systems.Create<UnapplyStatusesOfDeadTargetSystem>());
        }
    }
}