using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class CleanupCollectedSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _pullables;
        public CleanupCollectedSystem(GameContext game)
        {
            _pullables = game.GetGroup(GameMatcher.Collected);
        }

        public void Cleanup()
        {
            foreach (GameEntity pullable in _pullables)
            {
                pullable.isDestructed = true;
            }
        }
    }
}