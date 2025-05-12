using Code.Common.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Systems
{
    public class RotatingByDirectionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _rotaters;

        public RotatingByDirectionSystem(GameContext game)
        {
            _rotaters = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Direction,
                    GameMatcher.LastDirection,
                    GameMatcher.Transform, 
                    GameMatcher.Rotation));
        }

        public void Execute()
        {
            foreach (GameEntity rotater in _rotaters)
            {
                var dir = rotater.Direction;

                if (dir != Vector3.zero)
                {
                    rotater.ReplaceLastDirection(dir);
                }
                else if (rotater.hasLastDirection)
                {
                    dir = rotater.LastDirection;
                }
                else
                {
                    continue;
                }

                rotater.Transform.SmoothRotateToDirection(dir, rotater.Rotation);
            }
        }
    }
}