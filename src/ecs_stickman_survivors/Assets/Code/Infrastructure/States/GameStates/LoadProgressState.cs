using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Progress.SaveLoad;
using IState = Code.Infrastructure.States.StateInfrastructure.IState;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadProgressState : SimpleState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IStaticDataService _staticDataService;
        private readonly ISaveLoadService _saveLoadService;

        public LoadProgressState(
            IGameStateMachine stateMachine,
            IStaticDataService staticDataService,
            ISaveLoadService saveLoadService)
        {
            _stateMachine = stateMachine;
            _staticDataService = staticDataService;
            _saveLoadService = saveLoadService;
        }

        public override void Enter()
        {
            InitializeProgress();

            _stateMachine.Enter<ActualizeProgressState>();
        }

        private void InitializeProgress()
        {
            if(_saveLoadService.HasSaveProgress)
                _saveLoadService.LoadProgress();
            else 
                CreateNewProgress();
        }

        private void CreateNewProgress()
        {
            _saveLoadService.CreateProgress();

            CreateMetaEntity.Empty()
                .With(x => x.isStorage = true)
                .AddGold(0)
                .AddGoldPerSecond(_staticDataService.AfkGainConfig.GoldPerSecond);

            CreateMetaEntity.Empty()
                .With(x => x.isStorage = true)
                .AddGem(0)
                .AddGemPerSecond(_staticDataService.AfkGainConfig.GemPerSecond)
                .AddGemChance(_staticDataService.AfkGainConfig.GemChance);
        }
    }
}