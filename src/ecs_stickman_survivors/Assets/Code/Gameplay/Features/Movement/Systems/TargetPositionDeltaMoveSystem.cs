using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Movement.Systems
{
    public class TargetPositionDeltaMoveSystem : IExecuteSystem
    {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _movers;

        public TargetPositionDeltaMoveSystem(GameContext gameContext, ITimeService time)
        {
            _time = time;
            
            _movers = gameContext.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.WorldPosition,
                    GameMatcher.TargetPosition,
                    GameMatcher.Speed,
                    GameMatcher.MovementAvailable,
                    GameMatcher.Moving));
        }

        public void Execute()
        {
            var moversToStop = new List<GameEntity>();

            foreach (GameEntity mover in _movers)
            {
                Vector2 currentPosition = mover.WorldPosition;
                Vector2 targetPosition = mover.TargetPosition;
                float speed = mover.Speed;
                float deltaTime = _time.DeltaTime;

                Vector2 direction = (targetPosition - currentPosition).normalized;
                Vector2 newPosition = currentPosition + direction * speed * deltaTime;

                if (Vector2.Distance(newPosition, targetPosition) < speed * deltaTime)
                {
                    newPosition = targetPosition;
                    moversToStop.Add(mover); 
                }

                mover.ReplaceWorldPosition(newPosition);
            }
            
            foreach (var mover in moversToStop)
            {
                mover.isMoving = false;
            }
        }
    }

}