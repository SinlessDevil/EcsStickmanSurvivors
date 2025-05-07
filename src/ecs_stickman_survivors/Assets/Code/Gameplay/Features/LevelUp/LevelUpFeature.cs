using Code.Gameplay.Features.LevelUp.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.LevelUp
{
    public class LevelUpFeature : Feature
    {
        public LevelUpFeature(ISystemFactory system)
        {
            Add(system.Create<OpenLevelUpWindowSystem>());
            Add(system.Create<StopTimeOnLevelUpWindowSystem>());
            
            Add(system.Create<UpgradeAbilityRequestSystem>());

            Add(system.Create<StartTimeOnLevelUpWindowProcessedSystem>());
            
            Add(system.Create<FinalizeProcessedLevelUpSystem>());
        }
    }
}