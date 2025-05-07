using System.Collections.Generic;
using Code.Infrastructure.States.GameResultStates;

using Code.Infrastructure.States.StateMachine;
using Entitas;

namespace Code.Gameplay.GameOver.Systems
{
    public class GameOverOnHeroDeathSystem : ReactiveSystem<GameEntity>
    {
        private readonly IGameResultStateMachine _stateMachine;

        public GameOverOnHeroDeathSystem(GameContext game, IGameResultStateMachine stateMachine) : base(game)
        {
            _stateMachine = stateMachine;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
            context.CreateCollector(GameMatcher
                .AllOf(
                    GameMatcher.Hero,
                    GameMatcher.Dead)
                .Added());

        protected override bool Filter(GameEntity hero) => hero.isDead;

        protected override void Execute(List<GameEntity> heroes)
        {
            _stateMachine.Enter<GameOverState>();
        }
    }
}