using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.CharacterStats.Systems
{
    public sealed class StatFeature : Feature
    {
        public StatFeature(ISystemFactory systems)
        {
            Add(systems.Create<StatChangeSystem>());
            Add(systems.Create<ApplySpeedFromStatsSystem>());
            Add(systems.Create<ApplyHpFromStatsSystem>());
        }
    }
}