using Code.Gameplay.Common.Time;
using Entitas;

namespace Code.Meta.Features.Simulation
{
    public abstract class TimerExecuteSystem : IExecuteSystem
    {
        private readonly float _executeIntervalInSeconds;
        private readonly ITimeService _time;
        
        private float _timeToExecute;

        protected TimerExecuteSystem(float executeIntervalInSeconds, ITimeService time)
        {
            _executeIntervalInSeconds = executeIntervalInSeconds;
            _time = time;
        }

        protected abstract void Execute();

        void IExecuteSystem.Execute()
        {
            _timeToExecute -= _time.DeltaTime;
            if (_timeToExecute > 0) 
                return;
            
            _timeToExecute = _executeIntervalInSeconds;
            
            Execute();
        }
    }
}