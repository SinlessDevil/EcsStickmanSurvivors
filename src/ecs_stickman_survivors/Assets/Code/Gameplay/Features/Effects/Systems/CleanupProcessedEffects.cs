using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.Effects.Systems
{
    public class CleanupProcessedEffects : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _effects;
        private readonly List<GameEntity> _buffer = new(32);

        public CleanupProcessedEffects(GameContext gameContext)
        {
            _effects = gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.Effect, 
                GameMatcher.Processed));
        }

        public void Cleanup()
        {
            foreach (GameEntity effect in _effects.GetEntities(_buffer))
            {
                effect.Destroy();
            }
        }
    }
}