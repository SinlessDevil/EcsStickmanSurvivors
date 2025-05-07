using Entitas;

namespace Code.Meta.Features.Simulation.Systems
{
    public class BoosterDurationSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _boosters;
        private readonly IGroup<MetaEntity> _ticks;

        public BoosterDurationSystem(MetaContext game)
        {
            _boosters = game.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.GoldGainBoost, 
                    MetaMatcher.Duration));

            _ticks = game.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Tick));
        }

        public void Execute()
        {
            foreach (MetaEntity tick in _ticks)
            foreach (MetaEntity booster in _boosters)
            {
                booster.ReplaceDuration(booster.Duration - tick.Tick);
                
                if(booster.Duration <= 0)
                    booster.isDestructed = true;
            }
        }
    }
}