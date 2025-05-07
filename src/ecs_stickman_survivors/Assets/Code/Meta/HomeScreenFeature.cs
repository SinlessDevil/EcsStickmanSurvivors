using Code.Common.Destruct;
using Code.Infrastructure.Systems;
using Code.Meta.Features.Simulation;
using Code.Meta.Features.Simulation.Systems;
using Code.Progress;

namespace Code.Meta
{
    public class HomeScreenFeature : Feature
    {
        public HomeScreenFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<EmitTickSystem>(MetaConstants.SimulationTickSeconds));
            
            Add(systemFactory.Create<SimulationFeature>());
            Add(systemFactory.Create<HomeUIFeature>());
            
            Add(systemFactory.Create<PeriodicallySaveProgressSystem>(MetaConstants.SaveProgressSeconds)); //Test
            
            Add(systemFactory.Create<CleanupTickSystem>());
            Add(systemFactory.Create<ProcessDestructedFeature>());
        }
    }
}