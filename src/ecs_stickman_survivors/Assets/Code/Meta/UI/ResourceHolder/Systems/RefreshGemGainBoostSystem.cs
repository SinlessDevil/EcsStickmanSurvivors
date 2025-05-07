using System.Collections.Generic;
using Code.Meta.UI.GoldHolder.Service;
using Entitas;

namespace Code.Meta.UI.ResourceHolder.Systems
{
    public class RefreshGemGainBoostSystem : ReactiveSystem<MetaEntity> , IInitializeSystem
    {
        private readonly IStorageUIService _storageUIService;
        
        private readonly MetaContext _meta;
        private readonly IGroup<MetaEntity> _boosters;
        private readonly List<MetaEntity> _boostersBuffer = new(4);

        public RefreshGemGainBoostSystem(MetaContext meta,
            IStorageUIService storageUIService) : base(meta)
        {
            _storageUIService = storageUIService;
            _boosters = meta.GetGroup(MetaMatcher.GemGainBoost);
        }

        public void Initialize()
        {
            UpdateGoldGainBoost(_boosters.GetEntities(_boostersBuffer));
        }
        
        protected override ICollector<MetaEntity> GetTrigger(IContext<MetaEntity> context) =>
            context.CreateCollector(MetaMatcher.GemGainBoost.AddedOrRemoved());

        protected override bool Filter(MetaEntity booster) => true;

        protected override void Execute(List<MetaEntity> boosters)
        {
            UpdateGoldGainBoost(boosters);
        }

        private void UpdateGoldGainBoost(List<MetaEntity> boosters)
        {
            float goldGainBoost = 0f;
            
            foreach (MetaEntity booster in boosters)
            {
                if(booster.hasGemGainBoost)
                    goldGainBoost += booster.GemGainBoost;
            }
            
            _storageUIService.UpdateResourceBoost(ResourceTypeId.Gem, goldGainBoost);
        }
    }
}