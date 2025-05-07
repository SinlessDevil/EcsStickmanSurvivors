using Code.Infrastructure.Systems;
using Code.Meta.Features.Simulation.Systems;

namespace Code.Meta.Features.Simulation
{
    public sealed class SimulationFeature : Feature
    {
        public SimulationFeature(ISystemFactory systems)
        {
            Add(systems.Create<BoosterDurationSystem>());
            Add(systems.Create<CalculateGoldGainSystem>());
            Add(systems.Create<CalculateGemGainSystem>());
            
            Add(systems.Create<AfkGoldGainSystem>());
            Add(systems.Create<AfkGemGainSystem>());
            Add(systems.Create<UpdateSimulationTimeSystem>());
        }
    }
}