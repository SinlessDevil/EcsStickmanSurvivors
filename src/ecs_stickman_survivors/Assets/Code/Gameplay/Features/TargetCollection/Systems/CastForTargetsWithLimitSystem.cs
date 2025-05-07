using System;
using System.Collections.Generic;
using Code.Gameplay.Common.Physics;
using Entitas;

namespace Code.Gameplay.Features.TargetCollection.Systems
{
    public class CastForTargetsWithLimitSystem : IExecuteSystem , ITearDownSystem
    {
        private GameEntity[] _targetCastBuffer = new GameEntity[128];
        
        private readonly IPhysicsService _physicsService;
        private readonly IGroup<GameEntity> _ready;
        private readonly List<GameEntity> _buffer = new(64);
        
        public CastForTargetsWithLimitSystem(GameContext gameContext, IPhysicsService physicsService)
        {
            _physicsService = physicsService;
            _ready = gameContext.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.ReadyToCollectTargets,
                    GameMatcher.Radius,
                    GameMatcher.TargetsBuffer,
                    GameMatcher.ProcessedTargets,
                    GameMatcher.TargetLimit,
                    GameMatcher.WorldPosition,
                    GameMatcher.LayerMask
                ));
        }
        
        public void Execute()
        {
            foreach (GameEntity entity  in _ready.GetEntities(_buffer))
            {
                for (int i = 0; i < Math.Min(TargetCountInRadius(entity), entity.TargetLimit); i++)
                {
                    int targetId = _targetCastBuffer[i].Id;
                    if (!AlreadyProcessed(entity, targetId))
                    {
                        entity.TargetsBuffer.Add(targetId);
                        entity.ProcessedTargets.Add(targetId);
                    }
                }
                
                if(!entity.isCollectingTargetsContinuously) 
                    entity.isReadyToCollectTargets = false;
            }
        }

        public void TearDown()
        {
            _targetCastBuffer = null;
        }
        
        private bool AlreadyProcessed(GameEntity entity, int targetId)
        {
           return entity.ProcessedTargets.Contains(targetId);
        }
        
        private int TargetCountInRadius(GameEntity entity) =>
            _physicsService
                .CircleCastNonAlloc(entity.WorldPosition, entity.Radius, entity.LayerMask, _targetCastBuffer);
    }
}