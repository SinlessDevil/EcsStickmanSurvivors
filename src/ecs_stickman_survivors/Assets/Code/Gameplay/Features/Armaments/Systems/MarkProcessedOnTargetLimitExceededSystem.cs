using Entitas;

namespace Code.Gameplay.Features.Armaments.Systems
{
    public class MarkProcessedOnTargetLimitExceededSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _armaments;

        public MarkProcessedOnTargetLimitExceededSystem(GameContext game)
        {
            _armaments = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Armament,
                    GameMatcher.TargetLimit,
                    GameMatcher.ProcessedTargets));
        }

        public void Execute()
        {
            foreach (GameEntity armament in _armaments)
            {
                if(armament.ProcessedTargets.Count >= armament.TargetLimit)
                    armament.isProcessed = true;
            }
        }
    }
}