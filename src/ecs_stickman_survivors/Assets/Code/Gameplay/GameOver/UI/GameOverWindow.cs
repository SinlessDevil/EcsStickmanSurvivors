using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Services;
using Code.Infrastructure.States.GameResultStates;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.GameOver.UI
{
    public class GameOverWindow : BaseWindow
    {
        public Button ReturnHomeButton;

        private IGameStateMachine _gameStateMachine;
        private IGameResultStateMachine _gameResultStateMachine;
        private IWindowService _windowService;

        [Inject]
        private void Construct(IGameStateMachine stateMachine,
            IGameResultStateMachine gameResultStateMachine,
            IWindowService windowService)
        {
            Id = WindowId.GameOverWindow;

            _gameStateMachine = stateMachine;
            _gameResultStateMachine = gameResultStateMachine;
            _windowService = windowService;
        }

        protected override void Initialize()
        {
            ReturnHomeButton.onClick.AddListener(ReturnHome);
        }

        private void ReturnHome()
        {
            _windowService.Close(Id);

            _gameResultStateMachine.Enter<GameIdleState>();
            _gameStateMachine.Enter<LoadingHomeScreenState>();
        }
    }
}