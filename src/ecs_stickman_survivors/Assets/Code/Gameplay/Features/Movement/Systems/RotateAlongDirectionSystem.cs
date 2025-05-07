using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Systems
{
    public class RotateAlongDirectionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public RotateAlongDirectionSystem(GameContext game)
        {
            _entities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Transform,
                    GameMatcher.Direction,
                    GameMatcher.RotationAlignedByDirection));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                if (entity.Direction.sqrMagnitude >= 0.01f)
                {
                    var angle = Mathf.Atan2(entity.Direction.y, entity.Direction.x) * Mathf.Rad2Deg;
                    entity.Transform.rotation = Quaternion.Euler(0,0, angle);
                }
            }
        }
    }
}