using Code.Gameplay.Features.TargetCollection.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.TargetCollection
{
    public class CollectTargetFeature : Feature
    {
        public CollectTargetFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<CollectTargetIntervalSystem>());
            
            Add(systemFactory.Create<CastForTargetsNoLimitSystem>());
            Add(systemFactory.Create<CastForTargetsWithLimitSystem>());
            Add(systemFactory.Create<MarkReachedSystem>());
            
            Add(systemFactory.Create<CleanupTargetBuffersSystem>());
        }
    }
}