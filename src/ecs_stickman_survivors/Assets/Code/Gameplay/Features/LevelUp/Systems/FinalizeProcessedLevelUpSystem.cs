using Entitas;

namespace Code.Gameplay.Features.LevelUp
{
    public class FinalizeProcessedLevelUpSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _levelUps;

        public FinalizeProcessedLevelUpSystem(GameContext game)
        {
            _levelUps = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.LevelUp,
                    GameMatcher.Processed));
        }

        public void Cleanup()
        {
            foreach (GameEntity levelUp in _levelUps)
            {
                levelUp.isDestructed = true;
            }
        }
    }
}