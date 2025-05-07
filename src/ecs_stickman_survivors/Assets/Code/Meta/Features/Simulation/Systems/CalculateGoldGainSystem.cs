using Code.Gameplay.StaticData;
using Entitas;

namespace Code.Meta.Features.Simulation.Systems
{
    public class CalculateGoldGainSystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IGroup<MetaEntity> _boosters;
        private readonly IGroup<MetaEntity> _storages;

        public CalculateGoldGainSystem(MetaContext meta, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _boosters = meta.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.GoldGainBoost));

            _storages = meta.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Storage, MetaMatcher.GoldPerSecond));
        }

        public void Execute()
        {
            foreach (MetaEntity storage in _storages)
            {
                float gainBonus = 1;
                foreach (MetaEntity booster in _boosters)
                {
                    gainBonus += booster.GoldGainBoost;
                    storage.ReplaceGoldPerSecond(_staticDataService.AfkGainConfig.GoldPerSecond * gainBonus);
                }
            }
        }
    }
}