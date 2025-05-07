using Code.Gameplay.Features.LevelUp.Systems;
using Code.Gameplay.Features.Loot.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Loot
{
    public class LootingFeature : Feature
    {
        public LootingFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<CastForPullablesSytem>());
            
            Add(systemFactory.Create<PullTowardsHeroSystem>());
            Add(systemFactory.Create<CollectWhenNearSystem>());
            
            Add(systemFactory.Create<CollectExperienceSystem>());
            Add(systemFactory.Create<CollectEffectItemSystem>());
            Add(systemFactory.Create<CollectStatusItemSystem>());
            
            //UI Systems
            Add(systemFactory.Create<UpdateExperienceMeterSystem>());
            
            Add(systemFactory.Create<CleanupCollectedSystem>());
        }
    }
}