using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Code.Common.Extensions;
using Code.Gameplay.Common.Physics;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.Abilities.Upgrade;
using Code.Gameplay.Features.Armaments.Extensions;
using Code.Gameplay.Features.Armaments.Factory;
using Code.Gameplay.StaticData;
using Entitas;

namespace Code.Gameplay.Features.Armaments.Systems
{
    public class HandleScatteringAtTouchTargetSystem: IExecuteSystem
    {
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _armaments;
        
        private readonly List<GameEntity> _bufferArmaments = new(16);
        private readonly List<GameEntity> _bufferHeroes = new(1);
        
        private readonly GameContext _game;
        
        private readonly IArmamentFactory _armamentFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly IPhysicsService _physicsService;
        private readonly IAbilityUpgradeService _abilityUpgradeService;

        public HandleScatteringAtTouchTargetSystem (GameContext game, 
            IArmamentFactory armamentFactory,
            IStaticDataService staticDataService,
            IPhysicsService physicsService,
            IAbilityUpgradeService abilityUpgradeService)
        {
            _game = game;
            _armamentFactory = armamentFactory;
            _staticDataService = staticDataService;
            _physicsService = physicsService;
            _abilityUpgradeService = abilityUpgradeService;

            _armaments = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Armament,
                    GameMatcher.Separable,
                    GameMatcher.WorldPosition,
                    GameMatcher.Radius,
                    GameMatcher.LayerMask));
            
            _heroes = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Hero,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity hero in _heroes.GetEntities(_bufferHeroes))
            foreach (GameEntity armament in _armaments.GetEntities(_bufferArmaments))
            {
                if(armament.isSeparable == false)
                    continue;

                var targetId = TargetsInRadius(armament);
                
                if (targetId == 0)
                    continue;
                
                GameEntity currentTarget = _game.GetEntityWithId(targetId);

                int level = _abilityUpgradeService.GetAbilityLevel(AbilityId.ScatteringRuneStoneBolt);
                var abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.ScatteringRuneStoneBolt, level);
                
                for (int i = 0; i < abilityLevel.ProjectileSetup.ProjectileCount; i++)
                {
                    _armamentFactory
                        .CreateScatteringRuneStoneBolt(level, currentTarget.WorldPosition)
                        .AddProducerId(hero.Id)
                        .ReplaceDirection(i.GetDirectionByRadian(abilityLevel.ProjectileSetup.SpreadAngle,
                            abilityLevel.ProjectileSetup.ProjectileCount))
                        .With(x => x.isMoving = true)
                        .With(x => x.isSeparable = false);
                }

                armament.isSeparable = false;
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
    }
}