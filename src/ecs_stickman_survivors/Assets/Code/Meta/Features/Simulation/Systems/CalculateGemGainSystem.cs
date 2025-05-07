using Code.Gameplay.StaticData;
using Entitas;

namespace Code.Meta.Features.Simulation
{
    public class CalculateGemGainSystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        
        private readonly IGroup<MetaEntity> _boosters;
        private readonly IGroup<MetaEntity> _storages;

        public CalculateGemGainSystem(MetaContext meta, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            
            _boosters = meta.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.GemGainBoost));

            _storages = meta.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Storage, MetaMatcher.GemPerSecond));
        }

        public void Execute()
        {
            foreach (MetaEntity storage in _storages)
            {
                float gainBonus = 1;
                foreach (MetaEntity booster in _boosters)
                {
                    gainBonus += booster.GemGainBoost;
                    storage.ReplaceGemPerSecond(_staticDataService.AfkGainConfig.GemPerSecond * gainBonus);
                }
            }
        }
    }
}