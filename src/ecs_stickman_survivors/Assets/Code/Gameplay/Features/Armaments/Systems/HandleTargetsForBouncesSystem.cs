using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Common.Physics;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Armaments.Systems
{
    public class HandleTargetsForBouncesSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _armaments;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _heroes;

        private readonly GameContext _game;

        private readonly List<GameEntity> _bufferHero = new(1);
        private readonly List<GameEntity> _bufferArmaments = new(16);
        
        private readonly IPhysicsService _physicsService;

        public HandleTargetsForBouncesSystem(GameContext game, IPhysicsService physicsService)
        {
            _game = game;
            _physicsService = physicsService;

            _armaments = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Armament,
                    GameMatcher.Target,
                    GameMatcher.BounceRate,
                    GameMatcher.WorldPosition,
                    GameMatcher.Radius,
                    GameMatcher.LayerMask));

            _enemies = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Enemy,
                    GameMatcher.WorldPosition));

            _heroes = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Hero,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity hero in _heroes.GetEntities(_bufferHero))
            foreach (GameEntity armament in _armaments.GetEntities(_bufferArmaments))
            {
                // if (armament.BounceRate <= 0)
                //     continue;

                if (armament.hasTarget)
                {
                    var targetId = TargetsInRadius(armament);

                    if (targetId == 0)
                        continue;

                    GameEntity currentTarget = _game.GetEntityWithId(targetId);

                    if (currentTarget is { hasWorldPosition: true })
                    {
                        GameEntity newTarget = FindFarthestTarget(currentTarget);

                        if (newTarget != null)
                        {
                            armament.ReplaceTarget(newTarget.Id)
                                .ReplaceBounceRate(armament.BounceRate - 1)
                                .ReplaceDirection((newTarget.WorldPosition - hero.WorldPosition).normalized);
                        }
                    }
                }
            }
        }

        private int TargetsInRadius(GameEntity entity)
        {
            var targets = _physicsService
                .CircleCast(entity.WorldPosition, entity.Radius, entity.LayerMask)
                .OrderBy(t => Vector3.Distance(entity.WorldPosition, t.WorldPosition))
                .Select(t => t.Id)
                .ToList();

            return targets.FirstOrDefault();
        }

        private GameEntity FindFarthestTarget(GameEntity currentTarget)
        {
            float farthestDistanceSquared = 0;
            GameEntity farthestTarget = null;

            foreach (GameEntity enemy in _enemies)
            {
                if (enemy == currentTarget || enemy == null)
                    continue;
                
                float distanceSquared = (enemy.WorldPosition - currentTarget.WorldPosition).sqrMagnitude;

                if (distanceSquared > farthestDistanceSquared)
                {
                    farthestDistanceSquared = distanceSquared;
                    farthestTarget = enemy;
                }
            }

            return farthestTarget;
        }
    }
}