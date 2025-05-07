using Entitas;
using UnityEngine;

namespace Code.Meta.Features.Simulation.Systems
{
    public class AfkGemGainSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _ticks;
        private readonly IGroup<MetaEntity> _storages;

        public AfkGemGainSystem(MetaContext meta)
        {
            _ticks = meta.GetGroup(MetaMatcher.AllOf(MetaMatcher.Tick));
            _storages = meta.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.Storage,
                MetaMatcher.Gem,
                MetaMatcher.GemPerSecond,
                MetaMatcher.GemChance));
        }

        public void Execute()
        {
            foreach (MetaEntity tick in _ticks)
            foreach (MetaEntity storage in _storages)
            {
                if(storage.GemChance >= Random.Range(0, 1f)) 
                    storage.ReplaceGem(storage.Gem + tick.Tick * storage.GemPerSecond);
            }
        }
    }
}