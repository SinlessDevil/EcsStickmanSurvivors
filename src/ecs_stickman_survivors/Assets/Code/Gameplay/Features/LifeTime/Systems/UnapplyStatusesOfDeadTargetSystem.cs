using Entitas;

namespace Code.Gameplay.Features.LifeTime.Systems
{
    public class UnapplyStatusesOfDeadTargetSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _statuses;
        private readonly IGroup<GameEntity> _dead;

        public UnapplyStatusesOfDeadTargetSystem(GameContext game)
        {
            _statuses = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.TargetId,
                    GameMatcher.Status).NoneOf(GameMatcher.EnchantTypeId));
            
            _dead = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Id,
                    GameMatcher.Dead));
        }

        public void Execute()
        {
            foreach (GameEntity dead in _dead)
            foreach (GameEntity status in _statuses)
            {
                if (status.TargetId == dead.Id)
                    status.isUnapplied = true;   
            }
        }
    }
}