using Code.Gameplay.Features.EffectApplication.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.EffectApplication
{
    public class EffectApplicationFeature : Feature
    {
        public EffectApplicationFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ApplyEffectsOnTargetsSystem>());
            Add(systemFactory.Create<ApplyStatusOnTargetsAfterTouchTargetSystem>());
            Add(systemFactory.Create<ApplyStatusOnProducerAfterTouchTargetSystem>());
            Add(systemFactory.Create<ApplyStatusOnProducerAfterDeathTargetSystem>());
        }
    }
}