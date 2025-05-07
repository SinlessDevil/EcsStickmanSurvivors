using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Abilities.Upgrade;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Services;
using Code.Infrastructure.States.StateInfrastructure;

namespace Code.Infrastructure.States.GameResultStates
{
    public class GameOverState : SimpleState
    {
        private readonly IAbilityUpgradeService _abilityUpgradeService;
        private readonly IWindowService _windowService;
        private readonly ITimeService _timeService;

        public GameOverState(
            IAbilityUpgradeService abilityUpgradeService, 
            IWindowService windowService, 
            ITimeService timeService)
        {
            _abilityUpgradeService = abilityUpgradeService;
            _windowService = windowService;
            _timeService = timeService;
        }
    
        public override void Enter()
        {
            _timeService.StopTime();
            
            _abilityUpgradeService.Cleanup();
      
            _windowService.Open(WindowId.GameOverWindow);
        }

        protected override void Exit()
        {
            _timeService.StartTime();
        }
    }
}