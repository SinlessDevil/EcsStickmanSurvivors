using Code.Gameplay.Common.Time;
using Code.Meta.Features.Simulation;
using Code.Progress.SaveLoad;

namespace Code.Progress
{
    public class PeriodicallySaveProgressSystem : TimerExecuteSystem
    {
        private readonly ISaveLoadService _saveLoadService;

        public PeriodicallySaveProgressSystem(float executeIntervalInSeconds, 
            ITimeService time, 
            ISaveLoadService saveLoadService) : base(executeIntervalInSeconds, time)
        {
            _saveLoadService = saveLoadService;
        }


        protected override void Execute()
        {
            _saveLoadService.SaveProgress();
        }
    }
}