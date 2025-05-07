using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities.Upgrade;
using Code.Gameplay.Features.Armaments.Factory;
using Code.Gameplay.Features.Cooldowns;
using Code.Gameplay.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class OrbitingMushroomAbilitySystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IArmamentFactory _armamentFactory;
        private readonly IAbilityUpgradeService _abilityUpgradeService;

        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _heroes;
        
        private readonly List<GameEntity> _buffer = new(1);

        public OrbitingMushroomAbilitySystem(GameContext game, 
            IStaticDataService staticDataService, 
            IArmamentFactory armamentFactory,
            IAbilityUpgradeService abilityUpgradeService)
        {
            _staticDataService = staticDataService;
            _armamentFactory = armamentFactory;
            _abilityUpgradeService = abilityUpgradeService;

            _abilities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.OrbitingMushroomBoltAbility, 
                    GameMatcher.CooldownUp));

            _heroes = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Hero,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity ability in _abilities.GetEntities(_buffer))
            foreach (GameEntity hero in _heroes)
            {
                int level = _abilityUpgradeService.GetAbilityLevel(AbilityId.OrbitingMushroomBolt);
                var abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.OrbitingMushroomBolt, level);
                var projectileCount = abilityLevel.ProjectileSetup.ProjectileCount;
                
                for (int i = 0; i < projectileCount; i++)
                {
                    float phase = (2 * Mathf.PI * i / projectileCount);
                    
                    CreateProjectile(hero, phase, level);
                }
                    
                ability.PutOnCooldown(abilityLevel.Cooldown);
            }
        }

        private GameEntity CreateProjectile(GameEntity hero, float phase, int level)
        {
            return _armamentFactory
                .CreateOrbitingMushroomBolt(level, hero.WorldPosition + Vector3.up, phase)
                .AddProducerId(hero.Id)
                .AddOrbitCenterPosition(hero.WorldPosition)
                .AddOrbitCenterFollowTarget(hero.Id)
                .With(x => x.isMoving = true);
        }
    }
}