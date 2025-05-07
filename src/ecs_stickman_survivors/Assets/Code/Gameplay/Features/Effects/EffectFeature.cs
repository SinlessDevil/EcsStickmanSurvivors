using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Effects.Systems
{
    public sealed class EffectFeature : Feature
    {
        public EffectFeature(ISystemFactory systems)
        {
            Add(systems.Create<RemoveEffectsWithoutTargetsSystem>());
            
            Add(systems.Create<ProcessDamageEffectSystem>());
            Add(systems.Create<ProcessHealEffectSystem>());
            Add(systems.Create<ProcessSpeedEffectSystem>());
            
            Add(systems.Create<CleanupProcessedEffects>());
        }
    }
}